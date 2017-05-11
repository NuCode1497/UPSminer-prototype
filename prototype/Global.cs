// Copyright 2012 (C) Cody Neuburger  All rights reserved.
// This Control was written in a generic way to provide a user interface and functionality 
// to organize a mashed data table into a data set usable by a Graph object.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.CompilerServices;

namespace prototype
{
    public class Global
    {
        public static Form1 form1;
        public static List<string> errorStack = new List<string>();
        /// <summary>
        /// Generates error windows for any errors found on the error stack and clears the stack.
        /// </summary>
        public static void GenerateErrors()
        {
            while (errorStack.Count > 0)
            {
                Error err = new Error();
                err.label1.Text = errorStack[0];
                err.label2.Text = errorStack[1];
                errorStack.RemoveAt(1);
                errorStack.RemoveAt(0);
                if (err.label2.Text == "Deal with it.")
                {
                    err.pictureBox1.Image = Properties.Resources.deal;
                }
                err.ShowDialog();
            }
        }
        /// <summary>
        /// Generates a not yet implemented error.
        /// </summary>
        public static void NYI()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            errorStack.Add("Whoops! I haven't implented the " + sf.GetMethod().Name +
                " function yet. ");
            errorStack.Add("Deal with it.");
            GenerateErrors();
        }
        /// <summary>
        /// Generates a not yet implemented error.
        /// </summary>
        /// <param name="stack">Name of the function to refer to.</param>
        public static void NYI(string name)
        {
            errorStack.Add("Whoops! I haven't implented the " + name +
                " function yet. ");
            errorStack.Add("Deal with it.");
            GenerateErrors();
        }

        public static DateTime convertToDateTime(object o)
        {
            if (o.GetType() == typeof(float)) return DateTime.FromOADate((float)o);
            return DateTime.Parse(o.ToString());
        }
        /// <summary>
        /// Parses an input string for a data type and produces a typed variable.
        /// </summary>
        /// <param name="s">The input string to be parsed.</param>
        /// <param name="o">The output variable, bound to a data type.</param>
        /// <returns>The type of the variable.</returns>
        public static Type parseForType(string s, out object o)
        {
            DateTime t;
            bool isaDate = DateTime.TryParse(s, out t);
            if (isaDate)
            {
                o = t;
                return typeof(DateTime);
            }
            float f;
            bool isaFloat = float.TryParse(s, out f);
            if (isaFloat)
            {
                o = f;
                return typeof(float);
            }
            o = s;
            return typeof(string);
        }
        /// <summary>
        /// Parses each line of a file into a list.
        /// </summary>
        /// <param name="file">Input a file stream.</param>
        /// <returns>A list of lines from the file.</returns>
        public static List<string> ParseFile(Stream file)
        {
            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    String line;
                    List<string> list = new List<string>();
                    while ((line = sr.ReadLine()) != null)
                    {
                        list.Add(line);
                    }
                    return list;
                }
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
        /// <summary>
        /// Returns the name of the function in which this method was called.
        /// </summary>
        /// <returns>The name of the calling method.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }
        public static string getSafeFileName(string filename)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c.ToString(), "");
            }
            return filename;
        }
    }

    public class RowComparer : IEqualityComparer<DataRow>
    {
        public bool Equals(DataRow dr1, DataRow dr2)
        {
            if (dr1.ItemArray.Count() != dr2.ItemArray.Count()) return false;
            for (int i = 0; i < dr1.ItemArray.Count(); i++)
            {
                if (dr1[i].ToString() != dr2[i].ToString()) return false;
            }
            return true;
        }
        public int GetHashCode(DataRow dr)
        {
            int hCode = 0;
            for (int i = 0; i < dr.ItemArray.Count(); i++)
            {
                hCode += dr[i].ToString().GetHashCode();
            }
            return hCode.GetHashCode();
        }
    }
}
