// Copyright 2012 (C) Cody Neuburger  All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace prototype
{
    public partial class Graph : UserControl
    {
        #region Options
        /// <summary>
        /// The color of points and lines.
        /// </summary>
        public Color penColor;
        /// <summary>
        /// Toggle between OADate and date format.
        /// </summary>
        public bool dateCoordinates = true;
        /// <summary>
        /// Display coordinates on the cursor.
        /// </summary>
        public bool displayOnCursor = true;
        /// <summary>
        /// Use this string to display the coordinates under the cursor.
        /// </summary>
        public string coordinates;
        /// <summary>
        /// Use this string to display the Zoom level of the graph.
        /// </summary>
        public string zoomLevel;
        /// <summary>
        /// Toggle display of minimalistic grid lines. 
        /// </summary>
        public bool minimalGrid = false;
        /// <summary>
        /// Toggle display of grid lines.
        /// </summary>
        public bool showGrid = true;
        public float dotSize = 4f;
        /// <summary>
        /// Choose a method for displaying data on the graph. Use DotStyles for options.
        /// </summary>
        public DotStyles DotStyle;
        public enum DotStyles
        {
            DotsAndLines,
            Dots,
            Lines,
            MultiColor,
        }
        /// <summary>
        /// Toggle the ability to move the graph.
        /// </summary>
        public bool moveAndResizeMode = true;
        /// <summary>
        /// Called when the grid panel is painted.
        /// </summary>
        public EventHandler DidAnUpdate;
        public enum AxisFormats
        {
            Date,
            DateTime,
            Number,
            String,
        }
        /// <summary>
        /// Choose a format to display the axis labels.
        /// </summary>
        public AxisFormats XAxisformat;
        /// <summary>
        /// Choose a format to display the axis labels.
        /// </summary>
        public AxisFormats YAxisformat;
        public EventHandler CoordinatesChanged;
        public float ColorScale = 500;
        #endregion

        public Graph()
        {
            InitializeComponent();

            gridPanel.MouseWheel += new MouseEventHandler(gridPanel_MouseWheel);

            penColor = Color.Green;
            selection = gridPanel.ClientRectangle;
            GetPanelMaxMin(gridPanel);
            LoadData(null);
            XAxisformat = AxisFormats.Number;
            YAxisformat = AxisFormats.Number;
            DotStyle = DotStyles.DotsAndLines;

        }
        /// <summary>
        /// Loads a set of (X,Y) points that will be displayed on the graph. 
        /// </summary>
        /// <param name="Points">A list of (X,Y) points.</param>
        public void LoadData(List<PointF> Points)
        {
            state = STATE_POINTS;

            rmaxX = -10000000.0f;
            rminX = 10000000.0f;
            rmaxY = -10000000.0f;
            rminY = 10000000.0f;

            if (Points == null)
            {
                //default display of an arbitrary function
                orgPointList = new List<PointF>();
                for (int i = 0; i < 400; i++)
                {
                    PointF point = new PointF();
                    point.X = i;
                    point.Y = (float)(Math.Sin(Math.Sqrt((double)point.X))) * i * i;
                    orgPointList.Add(point);
                }
            }
            else if (Points.Count == 0)
            {
                MessageBox.Show("There's no data to graph!");
                return;
            }
            else
            {
                orgPointList = Points;
            }
            orgPointList.Sort(new ByX());
            pointList = new List<PointF>();
            foreach (PointF point in orgPointList)
            {
                setLimits(point);
                pointList.Add(point);
            }
            adjustLimits(ref rmaxX, ref rminX);
            adjustLimits(ref rmaxY, ref rminY);

            gmaxX = rmaxX;
            gmaxY = rmaxY;
            gminX = rminX;
            gminY = rminY;

            zoomStrength = 1.00f;
            zoomLevel = (int)(100 * zoomStrength) + "%";
            selection = gridPanel.ClientRectangle;
            selectionDefault = true;
            moveAndResizeMode = true;
            gridPanel.Invalidate();
            gridYLabelPanel.Invalidate();
            gridXLabelPanel.Invalidate();
        }
        /// <summary>
        /// Loads a DataTable into the graph using the specified columns for the X and Y axis.
        /// </summary>
        /// <param name="data">A DataTable object.</param>
        /// <param name="Xaxis">The column name of the table used to fetch the independent data that will be assigned to the X axis.</param>
        /// <param name="Yaxis">The column name of the table used to fetch the dependent data that will be assigned to the Y axis.</param>
        public void LoadData(DataTable data, string Xaxis, string Yaxis)
        {
            if (data.Rows.Count == 0)
            {
                MessageBox.Show("There's no data to graph!");
                return;
            }

            LoadDataStuff(data, Xaxis, Yaxis);

            state = STATE_TABLE;
        }
        /// <summary>
        /// Loads a DataTable into the graph using the specified columns for the X, Y, Z axis.
        /// </summary>
        /// <param name="data">A DataTable object.</param>
        /// <param name="Xaxis">The column name of the table used to fetch the independent data that will be assigned to the X axis.</param>
        /// <param name="Yaxis">The column name of the table used to fetch the dependent data that will be assigned to the Y axis.</param>
        /// <param name="Zaxis">The column name of the table used to fetch the dependent data that will be assigned to the Z axis.</param>
        public void LoadData(DataTable data, string Xaxis, string Yaxis, string Zaxis)
        {
            if (data.Rows.Count == 0)
            {
                MessageBox.Show("There's no data to graph!");
                return;
            }

            LoadDataStuff(data, Xaxis, Yaxis);

            Zcolumn = Zaxis;
            state = STATE_TABLE3;
            DotStyle = DotStyles.MultiColor;
            dotSize = 8;
        }
        private void LoadDataStuff(DataTable data, string Xaxis, string Yaxis)
        {
            rmaxX = -10000000.0f;
            rminX = 10000000.0f;
            rmaxY = -10000000.0f;
            rminY = 10000000.0f;

            dataTable = data;
            dataTable.Columns.Add("show");
            dataView = new DataView(dataTable);
            dataView.Sort = Yaxis;
            Xcolumn = Xaxis;
            Ycolumn = Yaxis;

            //X Axis
            Type type = data.Columns[Xaxis].DataType;
            if (type == typeof(DateTime))
            {
                XAxisformat = AxisFormats.Date;
            }
            else if (type == typeof(int))
            {
                XAxisformat = AxisFormats.Number;
            }
            else if (type == typeof(float))
            {
                XAxisformat = AxisFormats.Number;
            }
            else
            {
                XAxisformat = AxisFormats.String;
                using (DataTable UniqueXs = dataView.ToTable("UniqueXs", true, Xaxis))
                {
                    foreach (DataRowView row in UniqueXs.DefaultView)
                    {
                        XTranslateToFloat(row[Xaxis].ToString());
                    }
                }
            }

            //Y Axis (Must be a number type)
            YAxisformat = AxisFormats.Number;

            foreach (DataRowView row in dataView)
            {
                PointF point = new PointF(toFloat(row[Xcolumn]), toFloat(row[Ycolumn]));
                setLimits(point);
            }
            adjustLimits(ref rmaxX, ref rminX);
            adjustLimits(ref rmaxY, ref rminY);

            gmaxX = rmaxX;
            gmaxY = rmaxY;
            gminX = rminX;
            gminY = rminY;

            zoomStrength = 1.00f;
            zoomLevel = (int)(100 * zoomStrength) + "%";
            selection = gridPanel.ClientRectangle;
            selectionDefault = true;
            moveAndResizeMode = true;
            gridPanel.Invalidate();
            gridYLabelPanel.Invalidate();
            gridXLabelPanel.Invalidate();
        }
        /// <summary>
        /// Fit the entire graph to the panel.
        /// </summary>
        public void FullGraph()
        {
            zoomStrength = 1.00f;
            zoomLevel = (int)(100 * zoomStrength) + "%";
            selection = gridPanel.ClientRectangle;
            selectionDefault = true;
            gridPanel.Invalidate();
        }
        /// <summary>
        /// Stretch the graph and show more detail along the independant axis
        /// </summary>
        public void StretchHorizontal()
        {
            //editing rectangle
            graphSize = new SizeF(selection.Width, selection.Height);
            PointF pt = PointToClient(PointToScreen(Rectangle.Round(selection).Location));
            rect = new RectangleF(pt, graphSize);

            float numberOfPixelsToMove = 40 * zoomStrength;

            orgGraphLocation = selection.Location;
            selectionDefault = false;

            mouseStart = new Point(gridPanel.Width / 2, gridPanel.Height / 2);
            orgRelativeToGraph = new PointF(mouseStart.X - orgGraphLocation.X, mouseStart.Y - orgGraphLocation.Y);

            rect.Width += numberOfPixelsToMove;

            //stay above 100px
            if (Math.Min(rect.Width, rect.Height) < 100)
            {
                rect.Width = 100;

                //check if zoom is valid before drawing
                //set zoom
                zoomStrength = rect.Width / gridPanel.Width;
                if (zoomStrength <= maxZoomStrength && zoomStrength >= .001)
                {
                    zoomLevel = (int)(100 * zoomStrength) + "%";
                    selection = rect;
                }
            }
            else
            {
                //find % stretched
                float xStretched = ((float)rect.Width) / ((float)selection.Width);
                //stretch mouse point
                relativeToGraph.X = orgRelativeToGraph.X * xStretched;
                relativeToGraph.Y = orgRelativeToGraph.Y;
                //adjust graph to focus mouse point
                rect.X -= relativeToGraph.X - orgRelativeToGraph.X;
                rect.Y -= relativeToGraph.Y - orgRelativeToGraph.Y;

                //check if zoom is valid before drawing
                //set zoom
                zoomStrength = rect.Width / gridPanel.Width;
                if (zoomStrength <= maxZoomStrength && zoomStrength >= .001)
                {
                    zoomLevel = (int)(100 * zoomStrength) + "%";
                    selection = rect;
                }

            }
            gridPanel.Invalidate();
        }
        /// <summary>
        /// Shrink the graph and show less detail along the independant axis
        /// </summary>
        public void ShrinkHorizontal()
        {
            //editing rectangle
            graphSize = new SizeF(selection.Width, selection.Height);
            PointF pt = PointToClient(PointToScreen(Rectangle.Round(selection).Location));
            rect = new RectangleF(pt, graphSize);

            float numberOfPixelsToMove = -40 * zoomStrength;

            orgGraphLocation = selection.Location;
            selectionDefault = false;

            mouseStart = new Point(gridPanel.Width / 2, gridPanel.Height / 2);
            orgRelativeToGraph = new PointF(mouseStart.X - orgGraphLocation.X, mouseStart.Y - orgGraphLocation.Y);

            rect.Width += numberOfPixelsToMove;

            //stay above 100px
            if (rect.Width < 100)
            {
                rect.Width = 100;

                //check if zoom is valid before drawing
                //set zoom
                zoomStrength = rect.Width / gridPanel.Width;
                if (zoomStrength <= maxZoomStrength && zoomStrength >= .001)
                {
                    zoomLevel = (int)(100 * zoomStrength) + "%";
                    selection = rect;
                }
            }
            else
            {
                //find % stretched
                float xStretched = ((float)rect.Width) / ((float)selection.Width);
                //stretch mouse point
                relativeToGraph.X = orgRelativeToGraph.X * xStretched;
                relativeToGraph.Y = orgRelativeToGraph.Y;
                //adjust graph to focus mouse point
                rect.X -= relativeToGraph.X - orgRelativeToGraph.X;
                rect.Y -= relativeToGraph.Y - orgRelativeToGraph.Y;

                //check if zoom is valid before drawing
                //set zoom
                zoomStrength = rect.Width / gridPanel.Width;
                if (zoomStrength <= maxZoomStrength && zoomStrength >= .001)
                {
                    zoomLevel = (int)(100 * zoomStrength) + "%";
                    selection = rect;
                }

            }
            gridPanel.Invalidate();
        }
        /// <summary>
        /// Stretch the graph and show more detail along the dependant axis
        /// </summary>
        public void StretchVertical()
        {
            //editing rectangle
            graphSize = new SizeF(selection.Width, selection.Height);
            PointF pt = PointToClient(PointToScreen(Rectangle.Round(selection).Location));
            rect = new RectangleF(pt, graphSize);

            float numberOfPixelsToMove = 40 * zoomStrength;

            orgGraphLocation = selection.Location;
            selectionDefault = false;

            mouseStart = new Point(gridPanel.Width / 2, gridPanel.Height / 2);
            orgRelativeToGraph = new PointF(mouseStart.X - orgGraphLocation.X, mouseStart.Y - orgGraphLocation.Y);

            rect.Height += numberOfPixelsToMove;

            //stay above 100px
            if (rect.Height < 100)
            {
                rect.Height = 100;
                selection = rect;
            }
            else
            {
                //find % stretched
                float yStretched = ((float)rect.Height) / ((float)selection.Height);
                //stretch mouse point
                relativeToGraph.X = orgRelativeToGraph.X;
                relativeToGraph.Y = orgRelativeToGraph.Y * yStretched;
                //adjust graph to focus mouse point
                rect.X -= relativeToGraph.X - orgRelativeToGraph.X;
                rect.Y -= relativeToGraph.Y - orgRelativeToGraph.Y;
                selection = rect;

            }
            gridPanel.Invalidate();
        }
        /// <summary>
        /// Shrink the graph and show less detail along the dependant axis
        /// </summary>
        public void ShrinkVertical()
        {
            //editing rectangle
            graphSize = new SizeF(selection.Width, selection.Height);
            PointF pt = PointToClient(PointToScreen(Rectangle.Round(selection).Location));
            rect = new RectangleF(pt, graphSize);

            float numberOfPixelsToMove = -40 * zoomStrength;

            orgGraphLocation = selection.Location;
            selectionDefault = false;

            mouseStart = new Point(gridPanel.Width / 2, gridPanel.Height / 2);
            orgRelativeToGraph = new PointF(mouseStart.X - orgGraphLocation.X, mouseStart.Y - orgGraphLocation.Y);

            rect.Height += numberOfPixelsToMove;

            //stay above 100px
            if (rect.Height < 100)
            {
                rect.Height = 100;
                selection = rect;
            }
            else
            {
                //find % stretched
                float yStretched = ((float)rect.Height) / ((float)selection.Height);
                //stretch mouse point
                relativeToGraph.X = orgRelativeToGraph.X;
                relativeToGraph.Y = orgRelativeToGraph.Y * yStretched;
                //adjust graph to focus mouse point
                rect.X -= relativeToGraph.X - orgRelativeToGraph.X;
                rect.Y -= relativeToGraph.Y - orgRelativeToGraph.Y;
                selection = rect;

            }
            gridPanel.Invalidate();
        }
        public string Title
        {
            set
            {
                TitleLabel.Text = value;
            }
            get
            {
                return TitleLabel.Text;
            }
        }
        public string Title2
        {
            set
            {
                Title2Label.Text = value;
            }
            get
            {
                return Title2Label.Text;
            }
        }
        public string Title3
        {
            set
            {
                Title3Label.Text = value;
            }
            get
            {
                return Title3Label.Text;
            }
        }

        #region Variables
        DataTable dataTable;
        DataView dataView;
        string Xcolumn;
        string Ycolumn;
        string Zcolumn;
        List<PointF> orgPointList;
        List<PointF> pointList;
        List<PointF> corners;
        float rminX, rmaxX;
        float rminY, rmaxY;
        float gminX, gmaxX;
        float gminY, gmaxY;
        float sminX, smaxX;
        float sminY, smaxY;
        float xInterval;
        float yInterval;
        List<float> yLabels = new List<float>();
        List<float> xLabels = new List<float>();

        //selection
        RectangleF selection;
        bool selectionDefault = true;
        bool moveMode = false;
        bool resizeMode = false;
        bool mouseIsDown;
        PointF mouseStart;
        PointF mouseEnd;
        PointF relativeToGraph;
        PointF orgRelativeToGraph;
        PointF orgGraphLocation;
        PointF orgRectLocation;
        SizeF graphSize;
        RectangleF rect = Rectangle.Empty;
        float zoomStrength = 1.00f;
        float maxZoomStrength = 1000;

        int state = 0;
        const int STATE_POINTS = 0;
        const int STATE_TABLE = 1;
        const int STATE_TABLE3 = 2;
        #endregion

        private Dictionary<string, float> XStringToFloat = new Dictionary<string, float>();
        private Dictionary<float, string> XFloatToString = new Dictionary<float, string>();
        private Dictionary<string, float> YStringToFloat = new Dictionary<string, float>();
        private Dictionary<float, string> YFloatToString = new Dictionary<float, string>();
        private float fiter = 0;
        private float XTranslateToFloat(string s)
        {
            fiter++;
            if (!XStringToFloat.ContainsKey(s))
            {
                XFloatToString.Add(fiter, s);
                XStringToFloat.Add(s, fiter);
            }
            return fiter;
        }
        private float XTranslateFromString(string s)
        {
            return XStringToFloat[s];
        }
        private string XTranslateFromFloat(float f)
        {
            return XFloatToString[f];
        }
        private float YTranslateToFloat(string s)
        {
            fiter++;
            if (!YStringToFloat.ContainsKey(s))
            {
                YFloatToString.Add(fiter, s);
                YStringToFloat.Add(s, fiter);
            }
            return fiter;
        }
        private float YTranslateFromString(string s)
        {
            return YStringToFloat[s];
        }
        private string YTranslateFromFloat(float f)
        {
            return YFloatToString[f];
        }
        PointF transformToScreenPoint(PointF realPoint)
        {
            PointF screenPoint = new PointF();

            float rRangeX = rmaxX - rminX;
            float sRangeX = smaxX - sminX;
            screenPoint.X = (((realPoint.X - rminX) / rRangeX) * sRangeX) + sminX;

            float rRangeY = rmaxY - rminY;
            float sRangeY = smaxY - sminY;
            screenPoint.Y = (((realPoint.Y - rminY) / rRangeY) * sRangeY) + sminY;
            return screenPoint;
        }
        PointF transformToRealPoint(PointF screenPoint)
        {
            PointF realPoint = new PointF();

            float rRangeX = rmaxX - rminX;
            float sRangeX = smaxX - sminX;
            realPoint.X = (((screenPoint.X - sminX) / sRangeX) * rRangeX) + rminX;

            float rRangeY = rmaxY - rminY;
            float sRangeY = smaxY - sminY;
            realPoint.Y = (((screenPoint.Y - sminY) / sRangeY) * rRangeY) + rminY;
            return realPoint;
        }
        static void adjustLimits(ref float max, ref float min)
        {
            float numax, numin;
            if (max == min)
            {
                max++;
                min--;
            }
            float range = max - min;
            float interval = (float)Math.Pow(10.0, (Math.Floor(Math.Log10((double)range))) - 1.0f);
            if (max >= 0f)
            {
                for (numax = 0f; numax < max; numax += interval) { }
            }
            else
            {
                for (numax = 0f; numax > max; numax -= interval) { }
                numax += interval;
            }

            if (min <= 0f)
            {
                for (numin = 0f; numin > min; numin -= interval) { }
            }
            else
            {
                for (numin = 0f; numin < min; numin += interval) { }
                numin -= interval;
            }
            max = numax;
            min = numin;
        }
        private void setLimits(PointF point)
        {
            if (point.X > rmaxX) rmaxX = point.X;
            if (point.X < rminX) rminX = point.X;
            if (point.Y > rmaxY) rmaxY = point.Y;
            if (point.Y < rminY) rminY = point.Y;
        }
        private void cropGraphToPointList()
        {
            gmaxX = -10000000.0f;
            gminX = 10000000.0f;

            gmaxY = -10000000.0f;
            gminY = 10000000.0f;

            foreach (PointF point in pointList)
            {
                if (point.X > gmaxX) gmaxX = point.X;
                if (point.X < gminX) gminX = point.X;
                if (point.Y > gmaxY) gmaxY = point.Y;
                if (point.Y < gminY) gminY = point.Y;
            }
            foreach (PointF corner in corners)
            {
                float x = corner.X;
                float y = corner.Y;
                if (x > gmaxX) gmaxX = x;
                if (x < gminX) gminX = x;
                if (y > gmaxY) gmaxY = y;
                if (y < gminY) gminY = y;
            }
            adjustLimits(ref gmaxX, ref gminX);
            adjustLimits(ref gmaxY, ref gminY);
        }
        private void cropGraphToTable()
        {
            gmaxX = -10000000.0f;
            gminX = 10000000.0f;

            gmaxY = -10000000.0f;
            gminY = 10000000.0f;

            foreach (DataRowView point in dataView)
            {
                float x = toFloat(point[Xcolumn]);
                float y = toFloat(point[Ycolumn]);
                if (x > gmaxX) gmaxX = x;
                if (x < gminX) gminX = x;
                if (y > gmaxY) gmaxY = y;
                if (y < gminY) gminY = y;
            }
            foreach (PointF corner in corners)
            {
                float x = corner.X;
                float y = corner.Y;
                if (x > gmaxX) gmaxX = x;
                if (x < gminX) gminX = x;
                if (y > gmaxY) gmaxY = y;
                if (y < gminY) gminY = y;
            }
            adjustLimits(ref gmaxX, ref gminX);
            adjustLimits(ref gmaxY, ref gminY);
        }
        private void LabelHorizantalGridLine(Graphics g, float y)
        {
            Font font = new Font("Arial", 8);
            PointF rPoint = new PointF();
            PointF sPoint;
            rPoint.Y = y;
            rPoint.X = 0;
            sPoint = transformToScreenPoint(rPoint);
            sPoint.X = 65;
            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Far;
            strFormat.LineAlignment = StringAlignment.Center;
            String s = "";
            switch (YAxisformat)
            {
                case AxisFormats.Date:
                    DateTime dt = DateTime.FromOADate(y);
                    s = dt.ToString("yyyy MMM d");
                    break;
                case AxisFormats.DateTime:
                    DateTime dt2 = DateTime.FromOADate(y);
                    s = dt2.ToString("MMM d h:mmtt");
                    break;
                case AxisFormats.Number:
                case AxisFormats.String:
                    s = y.ToString();
                    break;
            }
            s += " ―";
            while (s.Length < 10) s = " " + s;
            g.DrawString(s, font, Brushes.Black, sPoint, strFormat);
        }
        private void LabelVerticalGridLine(Graphics g, float x)
        {
            Font font = new Font("Arial", 9);
            Pen pen = new Pen(new SolidBrush(Color.Black));
            PointF rPoint = new PointF();
            PointF sPoint;
            rPoint.Y = 0;
            rPoint.X = x;
            sPoint = transformToScreenPoint(rPoint);
            sPoint.Y = sPoint.X;
            sPoint.X = -5;
            PointF offset = new PointF(sPoint.X + 20, sPoint.Y);
            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Far;
            strFormat.LineAlignment = StringAlignment.Near;
            String s = "";
            switch (XAxisformat)
            {
                case AxisFormats.Date:
                    DateTime dt = DateTime.FromOADate(x);
                    s = dt.ToString("yyyy MMM d");
                    break;
                case AxisFormats.DateTime:
                    DateTime dt2 = DateTime.FromOADate(x);
                    s = dt2.ToString("MMM d h:mmtt");
                    break;
                case AxisFormats.Number:
                    s = x.ToString();
                    break;
                case AxisFormats.String:
                    s = XTranslateFromFloat(x);
                    break;
            }
            while (s.Length < 10) s = " " + s;
            g.RotateTransform(-90);
            g.DrawString(s, font, Brushes.Black, sPoint, strFormat);
            g.DrawLine(pen, offset, sPoint);
            g.RotateTransform(90);
        }
        private void DrawCoordinates(Graphics g, PointF p, Color c)
        {
            PointF pReal = transformToRealPoint(p);
            PointF offset = new PointF(p.X + 20, p.Y + 20);
            Font font = new Font("Times New Roman", 10, FontStyle.Bold);
            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Near;
            strFormat.LineAlignment = StringAlignment.Center;
            String s = "";
            switch (XAxisformat)
            {
                case AxisFormats.Date:
                    DateTime dt = DateTime.FromOADate(pReal.X);
                    s = dt.ToString("yyyy MMM d");
                    break;
                case AxisFormats.DateTime:
                    DateTime dt2 = DateTime.FromOADate(pReal.X);
                    s = dt2.ToString("yyyy MMM d h:mmtt");
                    break;
                case AxisFormats.Number:
                case AxisFormats.String:
                    s = pReal.X.ToString();
                    break;
            }
            if (dateCoordinates)
            {
                s = "(" + s + ", " + pReal.Y + ")";
            }
            else
            {
                s = "(" + pReal.X + ", " + pReal.Y + ")";
            }
            while (s.Length < 10) s = " " + s;

            if (displayOnCursor)
            {
                g.DrawString(s, font, new SolidBrush(c), offset, strFormat);
                g.DrawLine(new Pen(new SolidBrush(c)), offset, p);
            }
            coordinates = s;
            if (CoordinatesChanged != null) CoordinatesChanged(this, EventArgs.Empty);
        }
        private void DrawOneGridLine(Graphics g, Pen pen, float pos, int axis)
        {
            PointF start = new PointF();
            PointF end = new PointF();
            switch (axis)
            {
                case 1:
                    start.X = gminX;
                    start.Y = pos;
                    end.X = gmaxX;
                    end.Y = pos;
                    break;
                case 2:
                    start.Y = gminY;
                    start.X = pos;
                    end.Y = gmaxY;
                    end.X = pos;
                    break;
                default:
                    return;
            }
            PointF sStart = transformToScreenPoint(start);
            PointF sEnd = transformToScreenPoint(end);
            g.DrawLine(pen, sStart, sEnd);
        }
        private void drawHorizantalGridLines(Graphics g, bool MajorGridLines, Color col)
        {
            Pen pen = new Pen(col, 1.0f);
            Graphics gYLabel = gridYLabelPanel.CreateGraphics();
            float Yrange = rmaxY - rminY;
            float sYrange = sminY - smaxY;
            float interval;
            float high;
            float low;
            float labelMod;

            if (MajorGridLines)
            {
                interval = (float)Math.Pow(10.0, (Math.Floor(Math.Log10((double)Yrange))));
                high = gmaxY;
                low = gminY;

                //dynamic grid
                //shrinking
                while ((sYrange / (Yrange / interval)) < 100)
                {
                    interval *= 2;
                }
                //expanding
                while ((sYrange / (Yrange / interval)) > 200)
                {
                    interval /= 2;
                }

                //draw lines
                for (float y = 0.0f; y <= high; y += interval)
                {
                    if (y >= low)
                    {
                        DrawOneGridLine(g, pen, y, 1);
                    }
                }
                for (float y = 0.0f; y >= low; y -= interval)
                {
                    if (y <= high)
                    {
                        DrawOneGridLine(g, pen, y, 1);
                    }
                }
            }
            else
            {
                interval = (float)Math.Pow(10.0, (Math.Floor(Math.Log10((double)Yrange))) - 1.0f);
                yInterval = interval;
                high = gmaxY;
                low = gminY;
                labelMod = 2; //place a label every x lines

                //dynamic grid
                //shrinking
                while ((sYrange / (Yrange / interval)) < 10)
                {
                    interval *= 2;
                }
                //expanding
                while ((sYrange / (Yrange / interval)) > 20)
                {
                    interval /= 2;
                }

                //draw lines
                float labelPlacer = labelMod;
                yLabels.Clear();
                for (float y = 0.0f; y <= high; y += interval)
                {
                    if (y >= low)
                    {
                        if (labelPlacer == labelMod) yLabels.Add(y);
                        if (!minimalGrid) DrawOneGridLine(g, pen, y, 1);
                    }
                    labelPlacer--;
                    if (labelPlacer == 0) labelPlacer = labelMod;
                }
                labelPlacer = labelMod;
                for (float y = 0.0f; y >= low; y -= interval)
                {
                    if (y <= high)
                    {
                        if (labelPlacer == labelMod) yLabels.Add(y);
                        if (!minimalGrid) DrawOneGridLine(g, pen, y, 1);
                    }
                    labelPlacer--;
                    if (labelPlacer == 0) labelPlacer = labelMod;
                }
                gridYLabelPanel.Invalidate();
            }
        }
        private void drawVerticalGridLines(Graphics g, bool MajorGridLines, Color color)
        {
            Pen pen = new Pen(color, 1.0f);
            Graphics gXLabel = gridXLabelPanel.CreateGraphics();
            float Xrange = rmaxX - rminX;
            float sXrange = smaxX - sminX;
            float interval;
            float high;
            float low;
            float labelMod;

            if (MajorGridLines)
            {
                interval = (float)Math.Pow(10.0, (Math.Floor(Math.Log10((double)Xrange))));
                high = gmaxX;
                low = gminX;

                //dynamic grid
                //shrinking
                while ((sXrange / (Xrange / interval)) < 100)
                {
                    interval *= 2;
                }
                //expanding
                while ((sXrange / (Xrange / interval)) > 200)
                {
                    interval /= 2;
                }

                //draw lines
                for (float x = 0.0f; x <= high; x += interval)
                {
                    if (x >= low)
                    {
                        DrawOneGridLine(g, pen, x, 2);
                    }
                }
                for (float x = 0.0f; x >= low; x -= interval)
                {
                    if (x <= high)
                    {
                        DrawOneGridLine(g, pen, x, 2);
                    }
                }
            }
            else
            {
                interval = (float)Math.Pow(10.0, (Math.Floor(Math.Log10((double)Xrange))) - 1.0f);
                xInterval = interval;
                high = gmaxX;
                low = gminX;
                labelMod = 2; //place a label every x lines

                //dynamic grid
                //shrinking
                while ((sXrange / (Xrange / interval)) < 10)
                {
                    interval *= 2;
                }
                //expanding
                while ((sXrange / (Xrange / interval)) > 20)
                {
                    interval /= 2;
                }

                //draw lines
                float labelPlacer = labelMod;
                xLabels.Clear();
                for (float x = 0.0f; x <= high; x += interval)
                {
                    if (x >= low)
                    {
                        if (labelPlacer == labelMod) xLabels.Add(x);
                        if (!minimalGrid) DrawOneGridLine(g, pen, x, 2);
                    }
                    labelPlacer--;
                    if (labelPlacer == 0) labelPlacer = labelMod;
                }
                labelPlacer = labelMod;
                for (float x = 0.0f; x >= low; x -= interval)
                {
                    if (x <= high)
                    {
                        if (labelPlacer == labelMod) xLabels.Add(x);
                        if (!minimalGrid) DrawOneGridLine(g, pen, x, 2);
                    }
                    labelPlacer--;
                    if (labelPlacer == 0) labelPlacer = labelMod;
                }
                gridXLabelPanel.Invalidate();
            }
        }
        private void drawGridLines(Graphics g)
        {
            drawHorizantalGridLines(g, false, Color.LightBlue);
            drawHorizantalGridLines(g, true, Color.LightCoral);
            drawVerticalGridLines(g, false, Color.LightBlue);
            drawVerticalGridLines(g, true, Color.LightCoral);
        }
        private void DrawPoints(Graphics g)
        {
            Brush brush = new SolidBrush(penColor);
            Pen pen = new Pen(brush, 2.0f);
            Rectangle relevant = gridPanel.ClientRectangle;
            relevant.X -= 20;
            relevant.Y -= 20;
            relevant.Width += 40;
            relevant.Height += 40;
            g.DrawRectangle(pen, relevant);

            //cut out parts that arent shown
            pointList.Clear();
            PointF prevPoint = orgPointList[0];
            bool outside = true;
            foreach (PointF point in orgPointList)
            {
                PointF spoint = transformToScreenPoint(point);
                if (outside)
                {
                    if (relevant.Contains(Point.Round(spoint)) && !(pointList.Exists(p => p == point)))
                    {
                        pointList.Add(prevPoint);
                        pointList.Add(point);
                        outside = false;
                    }
                }
                else
                {
                    if (relevant.Contains(Point.Round(spoint)) && !(pointList.Exists(p => p == point)))
                    {
                        pointList.Add(point);
                    }
                    else
                    {
                        pointList.Add(point);
                        outside = true;
                    }
                }
                prevPoint = point;
            }

            //add points so the grid shows on the whole panel
            Point topRight = new Point(relevant.X + relevant.Width, relevant.Y);
            Point topLeft = new Point(relevant.X, relevant.Y);
            Point bottomRight = new Point(relevant.X + relevant.Width, relevant.Y + relevant.Height);
            Point bottomLeft = new Point(relevant.X, relevant.Y + relevant.Height);
            corners = new List<PointF>();
            corners.Add(transformToRealPoint(topRight));
            corners.Add(transformToRealPoint(topLeft));
            corners.Add(transformToRealPoint(bottomRight));
            corners.Add(transformToRealPoint(bottomLeft));

            //prepare for drawing graph
            cropGraphToPointList();
            if (showGrid) drawGridLines(g);

            switch (DotStyle)
            {
                case DotStyles.MultiColor:
                default:
                case DotStyles.DotsAndLines:
                    List<PointF> tempspointsList2 = new List<PointF>();
                    PointF[] tempspoints2;
                    foreach (PointF point in pointList)
                    {
                        tempspointsList2.Add(transformToScreenPoint(point));
                    }
                    tempspoints2 = tempspointsList2.ToArray();

                    try
                    {
                        //lines
                        g.DrawLines(pen, tempspoints2);
                        //dots
                        foreach (PointF point in tempspoints2)
                        {
                            float dotSizeZ = dotSize * zoomStrength;
                            if (zoomStrength > 6) dotSizeZ = dotSize * 6;
                            g.FillEllipse(brush, (float)(point.X - dotSizeZ / 2), (float)(point.Y - dotSizeZ / 2), dotSizeZ, dotSizeZ);
                        }
                    }
                    catch { }
                    break;
                case DotStyles.Dots:
                    List<PointF> tempspointsList = new List<PointF>();
                    PointF[] tempspoints;
                    foreach (PointF point in pointList)
                    {
                        tempspointsList.Add(transformToScreenPoint(point));
                    }
                    tempspoints = tempspointsList.ToArray();

                    try
                    {
                        //dots
                        foreach (PointF point in tempspoints)
                        {
                            float dotSizeZ = dotSize * zoomStrength;
                            if (zoomStrength > 6) dotSizeZ = dotSize * 6;
                            g.FillEllipse(brush, (float)(point.X - dotSizeZ / 2), (float)(point.Y - dotSizeZ / 2), dotSizeZ, dotSizeZ);
                        }
                    }
                    catch { }
                    break;
                case DotStyles.Lines:
                    List<PointF> tempspointsList3 = new List<PointF>();
                    PointF[] tempspoints3;
                    foreach (PointF point in pointList)
                    {
                        tempspointsList3.Add(transformToScreenPoint(point));
                    }
                    tempspoints3 = tempspointsList3.ToArray();

                    try
                    {
                        g.DrawLines(pen, tempspoints3);
                    }
                    catch { }
                    break;
            }
        }
        private void DrawTable(Graphics g)
        {
            SolidBrush brush = new SolidBrush(penColor);
            Pen pen = new Pen(brush, 2.0f);
            Rectangle relevant = gridPanel.ClientRectangle;
            relevant.X -= 20;
            relevant.Y -= 20;
            relevant.Width += 40;
            relevant.Height += 40;
            g.DrawRectangle(pen, relevant);

            //cut out parts that arent shown
            dataView = new DataView(dataTable);
            DataRowView prevPoint = dataView[0];
            bool outside = true;
            foreach (DataRowView point in dataView)
            {
                PointF spoint = new PointF(toFloat(point[Xcolumn]), toFloat(point[Ycolumn]));
                spoint = transformToScreenPoint(spoint);
                if (outside)
                {
                    if (!relevant.Contains(Point.Round(spoint)))
                    {
                        //flag row for removal
                        point["show"] = 0;
                    }
                    else
                    {
                        //flag row for keeps
                        point["show"] = 1;
                        prevPoint["show"] = 1;
                        outside = false;
                    }
                }
                else
                {
                    if (!relevant.Contains(Point.Round(spoint)))
                    {
                        //flag row for keeps
                        point["show"] = 1;
                        outside = true;
                    }
                    else
                    {
                        //flag row for keeps
                        point["show"] = 1;
                    }
                }
                prevPoint = point;
            }
            dataView.RowFilter = "show = 1";

            //add points so the grid shows on the whole panel
            Point topRight = new Point(relevant.X + relevant.Width, relevant.Y);
            Point topLeft = new Point(relevant.X, relevant.Y);
            Point bottomRight = new Point(relevant.X + relevant.Width, relevant.Y + relevant.Height);
            Point bottomLeft = new Point(relevant.X, relevant.Y + relevant.Height);
            corners = new List<PointF>();
            corners.Add(transformToRealPoint(topRight));
            corners.Add(transformToRealPoint(topLeft));
            corners.Add(transformToRealPoint(bottomRight));
            corners.Add(transformToRealPoint(bottomLeft));

            //prepare for drawing graph
            cropGraphToTable();
            if (showGrid) drawGridLines(g);

            switch (DotStyle)
            {
                case DotStyles.MultiColor:
                default:
                case DotStyles.DotsAndLines:
                    List<PointF> tempspointsList2 = new List<PointF>();
                    PointF[] tempspoints2;
                    foreach (DataRowView row in dataView)
                    {
                        PointF point = new PointF(toFloat(row[Xcolumn]), toFloat(row[Ycolumn]));
                        tempspointsList2.Add(transformToScreenPoint(point));
                    }
                    tempspoints2 = tempspointsList2.ToArray();

                    try
                    {
                        //lines
                        g.DrawLines(pen, tempspoints2);
                        //dots
                        foreach (PointF point in tempspoints2)
                        {
                            float dotSizeZ = dotSize * zoomStrength;
                            if (zoomStrength > 6) dotSizeZ = dotSize * 6;
                            g.FillEllipse(brush, (float)(point.X - dotSizeZ / 2), (float)(point.Y - dotSizeZ / 2), dotSizeZ, dotSizeZ);
                        }
                    }
                    catch { }
                    break;
                case DotStyles.Dots:
                    foreach (DataRowView row in dataView)
                    {
                        PointF point = new PointF(toFloat(row[Xcolumn]), toFloat(row[Ycolumn]));
                        point = transformToScreenPoint(point);
                        float dotSizeZ = dotSize * zoomStrength;
                        if (zoomStrength > 6) dotSizeZ = dotSize * 6;
                        g.FillEllipse(brush, (float)(point.X - dotSizeZ / 2), (float)(point.Y - dotSizeZ / 2), dotSizeZ, dotSizeZ);
                    }
                    break;
                case DotStyles.Lines:
                    List<PointF> tempspointsList3 = new List<PointF>();
                    PointF[] tempspoints3;
                    foreach (DataRowView row in dataView)
                    {
                        PointF point = new PointF(toFloat(row[Xcolumn]), toFloat(row[Ycolumn]));
                        tempspointsList3.Add(transformToScreenPoint(point));
                    }
                    tempspoints3 = tempspointsList3.ToArray();

                    try
                    {
                        g.DrawLines(pen, tempspoints3);
                    }
                    catch { }
                    break;
            }
        }
        private void DrawTable3(Graphics g)
        {
            SolidBrush brush = new SolidBrush(penColor);
            Pen pen = new Pen(brush, 2.0f);
            Rectangle relevant = gridPanel.ClientRectangle;
            relevant.X -= 20;
            relevant.Y -= 20;
            relevant.Width += 40;
            relevant.Height += 40;
            g.DrawRectangle(pen, relevant);

            //cut out parts that arent shown
            dataView = new DataView(dataTable);
            DataRowView prevPoint = dataView[0];
            bool outside = true;
            foreach (DataRowView point in dataView)
            {
                PointF spoint = new PointF(toFloat(point[Xcolumn]), toFloat(point[Ycolumn]));
                spoint = transformToScreenPoint(spoint);
                if (outside)
                {
                    if (!relevant.Contains(Point.Round(spoint)))
                    {
                        //flag row for removal
                        point["show"] = 0;
                    }
                    else
                    {
                        //flag row for keeps
                        point["show"] = 1;
                        prevPoint["show"] = 1;
                        outside = false;
                    }
                }
                else
                {
                    if (!relevant.Contains(Point.Round(spoint)))
                    {
                        //flag row for keeps
                        point["show"] = 1;
                        outside = true;
                    }
                    else
                    {
                        //flag row for keeps
                        point["show"] = 1;
                    }
                }
                prevPoint = point;
            }
            dataView.RowFilter = "show = 1";

            //add points so the grid shows on the whole panel
            Point topRight = new Point(relevant.X + relevant.Width, relevant.Y);
            Point topLeft = new Point(relevant.X, relevant.Y);
            Point bottomRight = new Point(relevant.X + relevant.Width, relevant.Y + relevant.Height);
            Point bottomLeft = new Point(relevant.X, relevant.Y + relevant.Height);
            corners = new List<PointF>();
            corners.Add(transformToRealPoint(topRight));
            corners.Add(transformToRealPoint(topLeft));
            corners.Add(transformToRealPoint(bottomRight));
            corners.Add(transformToRealPoint(bottomLeft));

            //prepare for drawing graph
            cropGraphToTable();
            if (showGrid) drawGridLines(g);

            switch (DotStyle)
            {
                default:
                case DotStyles.DotsAndLines:
                    List<PointF> tempspointsList2 = new List<PointF>();
                    PointF[] tempspoints2;
                    foreach (DataRowView row in dataView)
                    {
                        PointF point = new PointF(toFloat(row[Xcolumn]), toFloat(row[Ycolumn]));
                        tempspointsList2.Add(transformToScreenPoint(point));
                    }
                    tempspoints2 = tempspointsList2.ToArray();

                    try
                    {
                        //lines
                        g.DrawLines(pen, tempspoints2);
                        //dots
                        foreach (PointF point in tempspoints2)
                        {
                            float dotSizeZ = dotSize * zoomStrength;
                            if (zoomStrength > 6) dotSizeZ = dotSize * 6;
                            g.FillEllipse(brush, (float)(point.X - dotSizeZ / 2), (float)(point.Y - dotSizeZ / 2), dotSizeZ, dotSizeZ);
                        }
                    }
                    catch { }
                    break;
                case DotStyles.Dots:
                    foreach (DataRowView row in dataView)
                    {
                        PointF point = new PointF(toFloat(row[Xcolumn]), toFloat(row[Ycolumn]));
                        point = transformToScreenPoint(point);
                        float dotSizeZ = dotSize * zoomStrength;
                        if (zoomStrength > 6) dotSizeZ = dotSize * 6;
                        g.FillEllipse(brush, (float)(point.X - dotSizeZ / 2), (float)(point.Y - dotSizeZ / 2), dotSizeZ, dotSizeZ);
                    }
                    break;
                case DotStyles.Lines:
                    List<PointF> tempspointsList3 = new List<PointF>();
                    PointF[] tempspoints3;
                    foreach (DataRowView row in dataView)
                    {
                        PointF point = new PointF(toFloat(row[Xcolumn]), toFloat(row[Ycolumn]));
                        tempspointsList3.Add(transformToScreenPoint(point));
                    }
                    tempspoints3 = tempspointsList3.ToArray();

                    try
                    {
                        g.DrawLines(pen, tempspoints3);
                    }
                    catch { }
                    break;
                case DotStyles.MultiColor:
                    foreach (DataRowView row in dataView)
                    {
                        //dynamically determine the color based on Z axis
                        Color color = GetColorScale(0, ColorScale, toFloat(row[Zcolumn]));

                        PointF point = new PointF(toFloat(row[Xcolumn]), toFloat(row[Ycolumn]));
                        point = transformToScreenPoint(point);
                        float dotSizeZ = dotSize * zoomStrength;
                        if (zoomStrength > 6) dotSizeZ = dotSize * 6;
                        brush.Color = color;
                        g.FillEllipse(brush, (float)(point.X - dotSizeZ / 2), (float)(point.Y - dotSizeZ / 2), dotSizeZ, dotSizeZ);
                    }
                    break;
            }
        }
        private float toFloat(object o)
        {
            if (o.GetType() == typeof(DateTime))
            {
                return (float)((DateTime)o).ToOADate();
            }
            if (o.GetType() == typeof(float))
            {
                return (float)o;
            }
            try
            {
                return float.Parse(o.ToString());
            }
            catch
            {
                return 0;
            }
        }
        private object safeAssign(Type t, float o)
        {
            if (t == typeof(DateTime))
            {
                return DateTime.FromOADate((float)o);
            }
            if (t == typeof(float))
            {
                return (float)o;
            }
            return o.ToString();
        }
        private void GetPanelMaxMin(Panel p)
        {
            if (selectionDefault)
            {
                selection = p.ClientRectangle;
            }
            float edgeSize = 0.01f;
            sminX = selection.X;
            smaxY = selection.Y;
            smaxX = selection.X + selection.Width;
            sminY = selection.Y + selection.Height;
            if (smaxX < 100) smaxX = 100;
            if (sminY < 100) sminY = 100;
            float Xedge = (smaxX - sminX) * edgeSize;
            float Yedge = (sminY - smaxY) * edgeSize;
            sminX += Xedge;
            smaxX -= Xedge;
            sminY -= Yedge;
            smaxY += Yedge;
        }
        private Color GetColorScale(float rangeStart, float rangeEnd, float actualValue)
        {
            if (rangeStart >= rangeEnd) return Color.Black;
            if (actualValue >= rangeEnd) return Color.Red;

            float max = rangeEnd - rangeStart; // make the scale start from 0
            float value = actualValue - rangeStart; // adjust the value accordingly

            float red = (255 * value) / max;
            float blue = 255 - red;

            return Color.FromArgb((int)red, 0, (int)blue);
        }
        private void gridPanel_Paint(object sender, PaintEventArgs e)
        {
            GetPanelMaxMin(gridPanel);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            PointF relativeToPanel = new PointF(gridPanel.PointToClient(MousePosition).X, gridPanel.PointToClient(MousePosition).Y);

            g.FillRectangle(new SolidBrush(Color.AliceBlue), selection);

            switch (state)
            {
                case STATE_POINTS:
                    if (pointList.Count > 0) DrawPoints(g);
                    break;
                case STATE_TABLE:
                    DrawTable(g);
                    break;
                case STATE_TABLE3:
                    DrawTable3(g);
                    break;
            }

            Rectangle border = new Rectangle((int)selection.X - 1, (int)selection.Y - 1, (int)selection.Width + 2, (int)selection.Height + 2);
            g.DrawRectangle(new Pen(new SolidBrush(Color.BlueViolet), 1f), border);
            DrawCoordinates(g, relativeToPanel, Color.Black);
            g.FillRectangle(new SolidBrush(gridPanel.BackColor), 0, 0, border.Location.X, border.Location.Y + border.Height);
            g.FillRectangle(new SolidBrush(gridPanel.BackColor), 0, 0, border.Location.X + border.Width, border.Location.Y);
            g.FillRectangle(new SolidBrush(gridPanel.BackColor), border.Location.X + border.Width, 0, gridPanel.Width - (border.Location.X + border.Width), gridPanel.Height);
            g.FillRectangle(new SolidBrush(gridPanel.BackColor), 0, border.Location.Y + border.Height, gridPanel.Width, gridPanel.Height - (border.Y + border.Height));

            if (DidAnUpdate != null) DidAnUpdate(this, EventArgs.Empty);
        }
        private void gridPanel_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }
        private void gridYLabelPanel_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }
        private void gridYLabelPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            foreach (float y in yLabels)
            {
                LabelHorizantalGridLine(g, y);
            }
        }
        private void gridXLabelPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            foreach (float x in xLabels)
            {
                LabelVerticalGridLine(g, x);
            }
        }
        private void gridXLabelPanel_Resize(object sender, EventArgs e)
        {
            gridXLabelPanel.Invalidate();
        }
        private void gridPanel_MouseDown(object sender, MouseEventArgs e)
        {
            gridPanel.Focus();
            mouseStart = MousePosition;
            mouseIsDown = true;
            orgRectLocation = rect.Location;
            orgGraphLocation = selection.Location;

            //editing rectangle
            graphSize = new SizeF(selection.Width, selection.Height);
            PointF pt = PointToScreen(Rectangle.Round(selection).Location);
            rect = new RectangleF(pt, graphSize);

            if (moveAndResizeMode)
            {
                selectionDefault = false;
                if (e.Button == MouseButtons.Left)
                {
                    moveMode = true;
                    Cursor = Cursors.SizeAll;
                }
                if (e.Button == MouseButtons.Right)
                {
                    resizeMode = true;
                    Cursor = Cursors.NoMove2D;
                }
            }
        }
        private void gridPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseIsDown)
            {
                if (moveMode)
                {
                    mouseEnd = MousePosition;
                    rect.X = orgRectLocation.X + (mouseEnd.X - mouseStart.X);
                    rect.Y = orgRectLocation.Y + (mouseEnd.Y - mouseStart.Y);
                    PointF newPoint = new PointF();
                    newPoint.X = orgGraphLocation.X + (mouseEnd.X - mouseStart.X);
                    newPoint.Y = orgGraphLocation.Y + (mouseEnd.Y - mouseStart.Y);
                    selection.Location = newPoint;
                    gridPanel.Invalidate();
                }
                if (resizeMode)
                {
                    mouseEnd = MousePosition;
                    rect.Width = (mouseEnd.X - mouseStart.X) * 2 + graphSize.Width;
                    rect.Height = (mouseEnd.Y - mouseStart.Y) * 2 + graphSize.Height;
                    if (rect.Size.Width < 100) rect.Width = 100;
                    if (rect.Size.Height < 100) rect.Height = 100;
                    selection.Size = rect.Size;
                    selection.X = orgGraphLocation.X - (mouseEnd.X - mouseStart.X);
                    selection.Y = orgGraphLocation.Y - (mouseEnd.Y - mouseStart.Y);

                    //set zoom
                    zoomStrength = selection.Width / gridPanel.Width;
                    zoomLevel = (int)(100 * zoomStrength) + "%";
                }
            }
            gridPanel.Invalidate();
        }
        private void gridPanel_MouseUp(object sender, MouseEventArgs e)
        {
            mouseEnd = MousePosition;
            if (moveMode)
            {
                PointF newPoint = new PointF();
                newPoint.X = orgGraphLocation.X + (mouseEnd.X - mouseStart.X);
                newPoint.Y = orgGraphLocation.Y + (mouseEnd.Y - mouseStart.Y);
                selection.Location = newPoint;
                moveMode = false;
            }
            if (resizeMode)
            {
                if (rect.Width > 0 && rect.Height > 0)
                {
                    selection.Size = rect.Size;
                }
                //set zoom
                zoomStrength = selection.Width / gridPanel.Width;
                zoomLevel = (int)(100 * zoomStrength) + "%";

                resizeMode = false;
            }
            Cursor = Cursors.Default;
            mouseIsDown = false;
        }
        private void gridPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            //dont allow multiple actions at once
            if (!mouseIsDown)
            {
                //editing rectangle
                graphSize = new SizeF(selection.Width, selection.Height);
                PointF pt = PointToClient(PointToScreen(Rectangle.Round(selection).Location));
                rect = new RectangleF(pt, graphSize);

                // Update the drawing based upon the mouse wheel scrolling.
                int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
                float numberOfPixelsToMove = (numberOfTextLinesToMove * 20) * zoomStrength;
                float aspectRatio = (float)selection.Width / (float)selection.Height;

                orgGraphLocation = selection.Location;
                selectionDefault = false;

                mouseStart = gridPanel.PointToClient(MousePosition);
                orgRelativeToGraph = new PointF(mouseStart.X - orgGraphLocation.X, mouseStart.Y - orgGraphLocation.Y);

                rect.Width += numberOfPixelsToMove;
                rect.Height = (int)((float)rect.Width / aspectRatio);

                //keep aspect ratio and stay above 100px
                if (Math.Min(rect.Width, rect.Height) < 100)
                {
                    if (Math.Min(rect.Width / aspectRatio, rect.Height / aspectRatio) == rect.Width)
                    {
                        rect.Width = 100;
                        rect.Height = (int)(100 / aspectRatio);

                        //check if zoom is valid before drawing
                        //set zoom
                        zoomStrength = rect.Width / gridPanel.Width;
                        if (zoomStrength <= maxZoomStrength && zoomStrength >= .001)
                        {
                            zoomLevel = (int)(100 * zoomStrength) + "%";
                            selection = rect;
                        }
                    }
                    else
                    {
                        rect.Height = 100;
                        rect.Width = (int)(100 * aspectRatio);

                        //check if zoom is valid before drawing
                        //set zoom
                        zoomStrength = rect.Width / gridPanel.Width;
                        if (zoomStrength <= maxZoomStrength && zoomStrength >= .001)
                        {
                            zoomLevel = (int)(100 * zoomStrength) + "%";
                            selection = rect;
                        }
                    }
                }
                else
                {
                    //find % stretched
                    float xStretched = ((float)rect.Width) / ((float)selection.Width);
                    float yStretched = ((float)rect.Height) / ((float)selection.Height);
                    //stretch mouse point
                    relativeToGraph.X = orgRelativeToGraph.X * xStretched;
                    relativeToGraph.Y = orgRelativeToGraph.Y * yStretched;
                    //adjust graph to focus mouse point
                    rect.X -= relativeToGraph.X - orgRelativeToGraph.X;
                    rect.Y -= relativeToGraph.Y - orgRelativeToGraph.Y;

                    //check if zoom is valid before drawing
                    //set zoom
                    zoomStrength = rect.Width / gridPanel.Width;
                    if (zoomStrength <= maxZoomStrength && zoomStrength >= .001)
                    {
                        zoomLevel = (int)(100 * zoomStrength) + "%";
                        selection = rect;
                    }

                }
                gridPanel.Invalidate();
            }
        }
    }
    public class ByX : IComparer<PointF>
    {
        public ByX()
        {
        }
        public int Compare(PointF a, PointF b)
        {
            return a.X.CompareTo(b.X);
        }
    }
}
