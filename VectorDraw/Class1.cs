using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using nepumuk;


namespace nepumuk
{
    public class VectorDraw : System.Windows.Forms.PictureBox
    {

        // todo: after zoom/movecenter drawVectorAsLine new

        #region variables
        #region standard values
        /// <summary>
        /// origin of the drawing
        /// </summary>
        public Point zero;
        //definition of the axis
        private Vector xAchsis = new Vector(100, 0, 0);
        private Vector yAchsis = new Vector(0, -100, 0);
        private Vector zAchsis = new Vector(0, 0, 100);
        // colors
        private Color vectorColor;
        private Color axisColor;
        // some factors
        private double plainZfactor;
        private double zoomFactor;

        private float VectorSize = 0.5f;
        private float AxisSize = 0.5f;
        private Pen VectorPen;
        private Pen AxisPen;
        #endregion

        #region EventHandlingVars
        private bool LeftMouseClicked;
        private bool RightMouseClicked;
        private int mousex;
        private int mousey;
        #endregion

        #region Vars for storing the Vectors
        public ArrayList VectorAsPoint = new ArrayList();
        private ArrayList VectorAsLine = new ArrayList();
        private ArrayList VectorsAsTriangles = new ArrayList();
        private ArrayList VectorAsQuaders = new ArrayList();
        #endregion

        Graphics GraphicsVectorDraw;

        #region Errormessages
        private const string ERORR_CONVERTING_VECTOR_TO_POINT = "The dimension of the vector has to be 2";
        private const string ERROR_ZOOM_FACTOR_VALUE = "The value of the zoomfactor can't be zero or less.";
        private const string ERROR_DRAW_TRIANGLE_INVALID_ARGUMENTS = "The amount of vectors is not correct";
        #endregion
        #endregion

        #region Constructors and initialize
        public VectorDraw()
        {
            Initialize();

        }

        /// <summary>
        /// init the vectordraw, sets everything to an standardvalue
        /// </summary>
        public void Initialize()
        {
            try
            {
                // defines the center of the drawing in the center of the VectorDraw component
                zero.X = this.Width / 2;
                zero.Y = this.Height / 2;
                // create the graphics that has to bee drawn on
                GraphicsVectorDraw = this.CreateGraphics();
                // and the colors
                vectorColor = Color.Black;
                axisColor = Color.Black;
                // and the pens
                VectorPen = new Pen(vectorColor, VectorSize);
                AxisPen = new Pen(axisColor, AxisSize);
                plainZfactor = Math.PI / 4;
                zoomFactor = 1;
                
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        #endregion

        #region getter/setter
        /// <summary>
        /// get/set the backgroundcolor of the VectorDraw component
        /// </summary>
        public override Color BackColor
        {
            // pretty self explaining, isn't it?
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        /// <summary>
        /// get/set the color of the lines drawn on the VectorDraw Component
        /// </summary>
        public Color VectorColor
        {
            // pretty self explaining, isn't it?
            get { return vectorColor; }
            set { vectorColor = value; VectorPen = new Pen(vectorColor, 3); }
        }

        /// <summary>
        /// get/set the color of the Axis
        /// </summary>
        public Color AxisColor
        {
            get { return axisColor; }
            set { axisColor = value; }
        }

        /// <summary>
        /// sets/gets the value of zoom ( if ZF>1 -> image gets bigger)
        /// </summary>
        public double ZoomFactor
        {
            get { return zoomFactor; }
            set { if (value < Double.Epsilon) { throw new ArgumentException(ERROR_ZOOM_FACTOR_VALUE); } else { zoomFactor = value; } }
        }

        /// <summary>
        /// get/set the center of the drawing as a point
        /// </summary>
        public Point Center
        {
            get { return getCenter(); }
            set { zero = value; }
        }

        /// <summary>
        /// moves the center of the drawing with the defined parameters
        /// </summary>
        /// <param name="x">x offset</param>
        /// <param name="y">y offset</param>
        public void moveCenter(int x, int y)
        {
            try
            {
                // to move it with the given numbers => "+="
                zero.X += x;
                zero.Y += y;
                // Clear everything on the screen
                ClearDrawing();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// returns the central point of the drawing
        /// </summary>
        /// <returns>Point p</returns>
        public Point getCenter()
        {
            return zero;
        }

        #endregion


        #region organize vectordraw

        /// <summary>
        /// Refresh
        /// </summary>
        public override void Refresh()
        {
            zero.X = this.Width / 2;
            zero.Y = this.Height / 2;
            GraphicsVectorDraw = this.CreateGraphics();
            // base.Refresh();
        }

        /// <summary>
        /// Clear everything on the VectorDraw and drawVectorAsLine the x, y, z axis
        /// </summary>
        public void Clear()
        {
            // clears everything, and draws the x, y, z axis again
            try
            {
                GraphicsVectorDraw.Clear(BackColor);
                GraphicsVectorDraw.DrawLine(AxisPen, zero, VectorToPoint(plainVectorOnZ(xAchsis)));
                GraphicsVectorDraw.DrawLine(AxisPen, zero, VectorToPoint(plainVectorOnZ(yAchsis)));
                GraphicsVectorDraw.DrawLine(AxisPen, zero, VectorToPoint(plainVectorOnZ(zAchsis)));

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                // reset everything
                VectorAsPoint = new ArrayList();
                VectorAsLine = new ArrayList();
                VectorsAsTriangles = new ArrayList();
                VectorAsQuaders = new ArrayList();
            }

        }

        public void ClearDrawing()
        {
            GraphicsVectorDraw.Clear(BackColor);
            GraphicsVectorDraw.DrawLine(AxisPen, zero, VectorToPoint(plainVectorOnZ(xAchsis)));
            GraphicsVectorDraw.DrawLine(AxisPen, zero, VectorToPoint(plainVectorOnZ(yAchsis)));
            GraphicsVectorDraw.DrawLine(AxisPen, zero, VectorToPoint(plainVectorOnZ(zAchsis)));
        }

        /// <summary>
        /// redraws everything stored in the vectordraw
        /// </summary>
        public void reDraw()
        {
            try
            {
                foreach (Vector v in VectorAsPoint)
                {
                    drawPoint(v);
                }
            }

            catch (Exception e) { }
            try
            {
                foreach (Vector v in VectorAsLine)
                {
                    drawVectorAsLine(v);
                }
            }
            catch (Exception e) { }
           
                foreach (Vector[] vs in VectorsAsTriangles)
                {
                    this.drawTriangle(vs);
                }
           
            
            try
            {
                foreach (Vector[] vs in VectorAsQuaders)
                {
                    drawQuader(vs);
                }
            }
            catch (Exception e) { }
        }

        #endregion
               
        /// <summary>
        /// clears the VectorForm and draws the given vectors
        /// </summary>
        /// <param name="vectors"></param>
        public void clearDraw(Vector[] vectors)
        {
            try
            {
                // clear that damn thing
                Clear();
                // drawVectorAsLine all the vectors!
                drawVectorAsLine(vectors);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        /// <summary>
        /// clears everything and then draws the vector
        /// </summary>
        /// <param name="vector"></param>
        public void clearDraw(Vector vector)
        {
            try
            {
                // use the drawVectorAsLine[array of vectors] => don't need to program it again
                Vector[] v = { vector };
                clearDraw(v);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        #region container for draw-functions
        #region VectorAsLine

        /// <summary>
        /// draws all vectors in the given array from the middle of the drawing
        /// </summary>
        /// <param name="vectors">Arrays of vectors to be drawn</param>
        public void drawVectorAsLine(Vector[] vectors)
        {
            // make sure, every vector is drawn
            try
            {
                foreach (Vector cv in vectors)
                {
                    Vector v = new Vector(cv * zoomFactor);

                    // declare variables
                    double dxx = 0;
                    double dyy = 0;
                    double dz = v[2];
                    // because 3D vector is to be drawn on 2D
                    plainZ(dz, out dxx, out dyy);
                    double dx = v[0] - dxx / 2;
                    double dy = -v[1] + dyy / 2;
                    // finally drawVectorAsLine it
                    GraphicsVectorDraw.DrawLine(VectorPen, zero, new Point(Convert.ToInt32(dx) + zero.X, Convert.ToInt32(dy) + zero.Y));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///  draws all vectors in the given array from the middle of the drawing in the specific color. color switches back after drawing
        /// </summary>
        /// <param name="vectors">Vectors to drawVectorAsLine</param>
        /// <param name="vectorColor">color the vectors should have</param>
        public void drawVectorAsLine(Vector[] vectors, Color vectorColor)
        {
            // uses public void drawVectorAsLine(Vectors[] vectors)
            Color temp = this.vectorColor;
            this.vectorColor = vectorColor;
            drawVectorAsLine(vectors);
            vectorColor = temp;

        }

        /// <summary>
        /// drawVectorAsLine a sinlge vector
        /// </summary>
        /// <param name="vector">vector that has to be drawn</param>
        public void drawVectorAsLine(Vector vector)
        {
            try
            {
                // use the drawVectorAsLine[array of vectors] => don't need to program it again
                Vector[] v = { vector };
                drawVectorAsLine(v);
                //VectorAsPoint[1] = new Vector[VectorAsPoint[1].Count() + 1];
                //VectorAsPoint[1][VectorAsPoint[1].Count() + 1] = vector;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// draws the vector as a line from center to end of vector in the given color
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="vectorColor"></param>
        public void drawVectorAsLine(Vector vector, Color vectorColor)
        {
            // use the drawVectorAsLine[array of vectors] => don't need to program it again
            Vector[] v = { vector };
            drawVectorAsLine(v, vectorColor);
        }
        #endregion

        /// <summary>
        /// @@DEPRECIATED@@ draws a dice on the base of 3 Vectors
        /// it works, but you shouldn't use it!
        /// </summary>
        /// <param name="vectors"></param>
        public void drawQuader(Vector[] vectors)
        {

            this.ClearDrawing();
            Vector[] vs = vectors;
            // vs[0] = vs[0]*zoomFactor;
            //vs[1] =vs[0]*zoomFactor;
            //vs[2] =vs[0] * zoomFactor;
            Vector v1, v2, v3, v4, v5, v6, v7;
            v1 = plainVectorOnZ(new Vector(vs[0] * zoomFactor));
            v2 = plainVectorOnZ(new Vector(vs[1] * zoomFactor));
            v3 = plainVectorOnZ(new Vector(vs[2] * zoomFactor));
            v5 = plainVectorOnZ(new Vector(vs[0] * zoomFactor + vs[1] * zoomFactor));
            v6 = plainVectorOnZ(new Vector(vs[0] * zoomFactor + vs[2] * zoomFactor));
            v7 = plainVectorOnZ(new Vector(vs[0] * zoomFactor + vs[1] * zoomFactor + vs[2] * zoomFactor));
            v4 = plainVectorOnZ(new Vector(vs[2] * zoomFactor + vs[1] * zoomFactor));

            Vector[] dada = { v1, v2, v3 };
            Pen black = new Pen(Color.Black, 0.5f);
            //clearDraw(dada);
            GraphicsVectorDraw.DrawLine(black, VectorToPoint(new Vector(0, 0)), VectorToPoint(v1));
            GraphicsVectorDraw.DrawLine(black, VectorToPoint(new Vector(0, 0)), VectorToPoint(v2));
            GraphicsVectorDraw.DrawLine(black, VectorToPoint(new Vector(0, 0)), VectorToPoint(v3));
            GraphicsVectorDraw.DrawLine(black, VectorToPoint(v1), VectorToPoint(v5));
            GraphicsVectorDraw.DrawLine(black, VectorToPoint(v1), VectorToPoint(v6));
            GraphicsVectorDraw.DrawLine(black, VectorToPoint(v2), VectorToPoint(v5));
            GraphicsVectorDraw.DrawLine(black, VectorToPoint(v2), VectorToPoint(v4));
            GraphicsVectorDraw.DrawLine(black, VectorToPoint(v3), VectorToPoint(v4));
            GraphicsVectorDraw.DrawLine(black, VectorToPoint(v3), VectorToPoint(v6));
            GraphicsVectorDraw.DrawLine(black, VectorToPoint(v5), VectorToPoint(v7));
            GraphicsVectorDraw.DrawLine(black, VectorToPoint(v6), VectorToPoint(v7));
            GraphicsVectorDraw.DrawLine(black, VectorToPoint(v4), VectorToPoint(v7));

        }

        /// <summary>
        /// draws a triangle with the given vectors. make sure the array contains only 3 vectors
        /// </summary>
        /// <param name="vectors"></param>
        public void drawTriangle(Vector[] vectors)
        {
            try
            {
                if (vectors.Length != 3)
                {
                    throw new ArgumentException(ERROR_DRAW_TRIANGLE_INVALID_ARGUMENTS);
                }
                else
                {
                    drawLineBetweenVectors(vectors[0], vectors[1]);
                    drawLineBetweenVectors(vectors[1], vectors[2]);
                    drawLineBetweenVectors(vectors[0], vectors[2]);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// draws a line between the ends of the given vectors
        /// </summary>
        /// <param name="v1">start point</param>
        /// <param name="v2">end point</param>
        public void drawLineBetweenVectors(Vector v1, Vector v2)
        {
            GraphicsVectorDraw.DrawLine(VectorPen, VectorToPoint(v1), VectorToPoint(v2));
        }

        /// <summary>
        /// Draws a single point at the end of the given vector
        /// </summary>
        /// <param name="v"></param>
        public void drawPoint(Vector v)
        {
            try
            {
                Vector temp = new Vector(plainVectorOnZ(v));
                temp *= zoomFactor;
                GraphicsVectorDraw.DrawLine(VectorPen, new Point((int)temp[0] + 1 + zero.X, (int)temp[1] + 1 + zero.Y), new Point((int)temp[0] - 1 + zero.X, (int)temp[1] - 1 + zero.Y));
                GraphicsVectorDraw.DrawLine(VectorPen, new Point((int)temp[0] + 1 + zero.X, (int)temp[1] - 1 + zero.Y), new Point((int)temp[0] - 1 + zero.X, (int)temp[1] + 1 + zero.Y));
            }
            catch (Exception e)
            { }
        }

        public void drawCircle(Vector v1, Vector v2, Vector v3)
        {
            //GraphicsVectorDraw
            
        }

        /// <summary>
        /// draws a circle around zero with radius v1 orthogonal to v2
        /// </summary>
        /// <param name="v1">defines the radius of the circle</param>
        /// <param name="v2">orthogonal to this vector the circle is drawn</param>
        public void drawCircle(Vector v1, Vector v2)
        {
            drawCircle(Vector.origin, v1, v2);
        }
        #endregion 
        
        #region functions to add things to the drawing
        #region points
        /// <summary>
        /// Adds a point to the vectordraw that automatically gets redrawn when zoom and the central point is changed
        /// </summary>
        /// <param name="v">Vector that defines the point</param>
        public void addPointToDrawing(Vector v)
        {
            VectorAsPoint.Add(v);
        }
        /// <summary>
        /// Adds a number of points to the vectordraw that automatically gets redrawn when zoom and the central point is changed
        /// </summary>
        /// <param name="vs">Vectors that defines the point</param>
        public void addPointToDrawing(Vector[] vs)
        {

            foreach (Vector v in vs)
            {
                addPointToDrawing(v);
            }
        }
        #endregion
        #region lines
        /// <summary>
        /// Adds a line to the vectordraw that automatically gets redrawn when zoom and the central point is changed
        /// </summary>
        /// <param name="v">Vector that defines the Line of the Vector</param>
        public void addVectorAsLineToDrawing(Vector v)
        {
            VectorAsLine.Add(v);
        }
        /// <summary>
        /// Adds a lines to the vectordraw that automatically gets redrawn when zoom and the central point is changed
        /// </summary>
        /// <param name="vs"></param>
        public void addVectorAsLineToDrawing(Vector[] vs)
        {
            foreach (Vector v in vs)
            {
                addVectorAsLineToDrawing(v);
            }
        }
        #endregion
        #region triangle
        /// <summary>
        /// Adds a array of vectors to the vectordraw that automatically gets redrawn when zoom and the central point is changed
        /// </summary>
        /// <param name="vs"></param>
        public void addVectorAsTriangle(Vector[] vs)
        {
            VectorsAsTriangles.Add(vs);
        }
        #endregion
        #region quader
        public void addVectorAsQuader(Vector[] vs)
        {
            VectorAsQuaders.Add(vs);
        }
        #endregion
        #endregion

        #region Utilities to drawVectorAsLine
        /// <summary>
        /// converts z value to x and y value
        /// </summary>
        /// <param name="z"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void plainZ(double z, out double x, out double y)
        {
            x = Math.Sin(plainZfactor) * z;
            y = Math.Cos(plainZfactor) * z;
        }

        /// <summary>
        /// converts a 3D vector to a 2D vector
        /// </summary>
        /// <param name="v">Vector to be plained</param>
        /// <returns>plained vector</returns>
        private Vector plainVectorOnZ(Vector v)
        {
            double dxx = 0;
            double dyy = 0;
            double dz = v[2];
            // plain
            plainZ(dz, out dxx, out dyy);

            double dx = v[0] - dxx / 2;
            double dy = v[1] + dyy / 2;
            return new Vector(dx, dy);
        }

        /// <summary>
        /// converts an vector into a point, the vector away from origin
        /// </summary>
        /// <param name="v">Vector</param>
        /// <returns>point of a 2D vector</returns>
        public Point VectorToPoint(Vector v)
        {
            if (v.Dimension == 2)
            {
                return new Point(Convert.ToInt32(v[0]) + zero.X, Convert.ToInt32(v[1]) + zero.Y);
            }
            else
            {
                throw new ArgumentException(ERORR_CONVERTING_VECTOR_TO_POINT);
            }
        }
        #endregion

        #region EventHandling
        protected override void OnMouseDown(MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    LeftMouseClicked = true;
                    break;
                case MouseButtons.Right:
                    RightMouseClicked = true;
                    break;
            }

            mousex = e.X;
            mousey = e.Y;
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (LeftMouseClicked)
            {
                this.moveCenter(-mousex + e.X, -mousey + e.Y);
                mousex = e.X;
                mousey = e.Y;
                reDraw();
            }
            if (RightMouseClicked)
            {
                if ((-mousey + e.Y) / 10 < Double.Epsilon)
                {
                    this.ZoomFactor = 0.0001;
                }
                else
                {

                    this.ZoomFactor = Math.Abs(-mousey + e.Y) / 10;
                    reDraw();
                }
            }
            
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            LeftMouseClicked = false;
            RightMouseClicked = false;
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Refresh();
            base.OnSizeChanged(e);
        }
        #endregion

    }
}
