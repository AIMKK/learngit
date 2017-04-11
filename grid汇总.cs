.net DataGridView不支持汇总行，使用起来多有不便，本想重写此控件，
以前做Web项目时曾用Javascript写过与WinForm中的DataGridView类似的控件，
但分析微软的代码后感觉重写非常的烦，所以采用改造原控件的方式，
重写OnLayout函数，
当布局完毕，通过反射修改网络区的大小，
在底部留出汇总条的位置，将汇总条嵌在其中，
这样网格内容与汇总条内容不会相互影响
，同时在涉及布局和滚动的事件中对汇总行的位置尺寸重新计算，
以保证与网格与汇总条对齐。

汇总行使用Panel包含的ListView来显示汇总行内容，
考虑列冻结的情况，leftPanel用来显示冻结内容
rightPanel显示可滚动内容，汇总行在网络主体添加列时生成相应的对，
默认无汇总动作，对汇总行列的 SumMode 设置相应的值可启用汇总、计数、平均值等动作（程序需补充），
代码如下，希望对需要的人有所帮助

全部是text，这样可以的吗？


如果这个有，另外一个也必须要有的了，







using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using System.Collections;

namespace Nauja.GridFooter
{
    public partial class DataGridViewEx : DataGridView
    {
        private DataGridFooter _Footer = new DataGridFooter();

        public DataGridFooter Footer
        {
            get { return _Footer; }
            set { _Footer = value; }
        }
        public bool FooterVisible
        {
            get { return _Footer.Visible; }
            set { _Footer.Visible = value; }
        }
       

        public DataGridViewEx()
        {
            this.Controls.Add(this._Footer);
            
            this.Scroll += new ScrollEventHandler(DataGridViewEx_Scroll);
            this.ColumnWidthChanged += new DataGridViewColumnEventHandler(DataGridViewEx_ColumnWidthChanged);
            this.CellBeginEdit += new DataGridViewCellCancelEventHandler(DataGridViewEx_CellBeginEdit);
            this.CellEndEdit += new DataGridViewCellEventHandler(DataGridViewEx_CellEndEdit);
            this.ColumnAdded+=new DataGridViewColumnEventHandler(DataGridViewEx_ColumnAdded);
            this.RowHeadersWidthChanged += new EventHandler(DataGridViewEx_RowHeadersWidthChanged);
            this.SizeChanged += new EventHandler(DataGridViewEx_SizeChanged);
        }

        void DataGridViewEx_SizeChanged(object sender, EventArgs e)
        {
            RefreshFooter();
        }

        void DataGridViewEx_RowHeadersWidthChanged(object sender, EventArgs e)
        {
            _Footer.RowHeaderWidth = this.RowHeadersWidth;
            RefreshFooter();
        }

        void DataGridViewEx_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            _Footer.Columns.Add(new DataGridFooterColumn(DataGridFooterSumMode.None));

            RefreshFooter();
        }


        private object cellTempValue=0;

        void DataGridViewEx_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            cellTempValue = this[e.ColumnIndex, e.RowIndex].Value;
        }

        void DataGridViewEx_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (this[e.ColumnIndex,e.RowIndex].Value != cellTempValue)
            {
                if (_Footer.Columns[e.ColumnIndex].SumMode == DataGridFooterSumMode.Sum)
                {
                    decimal v=0, t=0;
                    try
                    {
                        t = ("" + cellTempValue == "" ? (decimal)0 : decimal.Parse("" + cellTempValue));
                        v = ("" + this[e.ColumnIndex, e.RowIndex].Value == "" ? (decimal)0 : decimal.Parse("" + this[e.ColumnIndex, e.RowIndex].Value));
                    }
                    catch (FormatException)
                    {
                        this[e.ColumnIndex, e.RowIndex].Value = "";
                        v = 0;
                    }
                    _Footer.Columns[e.ColumnIndex].Value = (decimal)_Footer.Columns[e.ColumnIndex].Value - t + v;
                }
                cellTempValue = 0;
            }
        }

        void DataGridViewEx_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            _Footer.Columns[e.Column.Index].Width = e.Column.Width;
            RefreshFooter();
        }
        private int BorderWidth
        {
            get { return 1; }
        }
        private void RefreshFooter()
        {
            Rectangle rect = new Rectangle();
            if (HorizontalScrollBar.Visible)
            {
                rect.X = BorderWidth;
                rect.Y = this.Height - _Footer.Height - HorizontalScrollBar.Height - BorderWidth;
                rect.Width = this.Width - BorderWidth * 2;
                rect.Height = _Footer.Height;
                _Footer.LayoutControl(rect);
            }
            else
            {
                rect.X = BorderWidth;
                rect.Y = this.Height - _Footer.Height - BorderWidth;
                rect.Width = this.Width - BorderWidth * 2;
                rect.Height = _Footer.Height;
                _Footer.LayoutControl(rect);
            }
            _Footer.Refresh();
        }


        void DataGridViewEx_Scroll(object sender, ScrollEventArgs e)
        {
            _Footer.ScrollWithParent(this.HorizontalScrollBar.Value);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            Type t = this.GetType();
            FieldInfo tLayout = t.BaseType.GetField("layout", BindingFlags.NonPublic | BindingFlags.Instance);
            if (tLayout == null) 
                return;

            Object layout = tLayout.GetValue(this);
            
            if (layout == null) 
                return;
            
            FieldInfo tDataClient = layout.GetType().GetField("Data");
            Rectangle dataClient = (Rectangle)tDataClient.GetValue(layout);
            if(this.ColumnHeadersHeight+(this.HorizontalScrollBar.Visible?this.HorizontalScrollBar.Height:0)+dataClient.Height>this.Height-this.HorizontalScrollBar.Height){
                dataClient = new Rectangle(dataClient.X, dataClient.Y, dataClient.Width, dataClient.Height - this.HorizontalScrollBar.Height);
                tDataClient.SetValue(layout, dataClient);
                RefreshFooter();
            }
        }
    }

    public class DataGridFooter:Panel
    {
        private ImageList _ImgList = new ImageList();
        private Panel _LeftPanel=new Panel();
        private Panel _RightPanel=new Panel();
        internal ListView _LeftView=new ListView();
        internal ListView _RightView = new ListView();
        private int _Frozen = 0;
        private DataGridViewEx _DataGridViewEx = null;

        public DataGridFooterColumnCollection Columns =null;
           

        public DataGridViewEx DGViewEx
        {
            get { return _DataGridViewEx; }
            set { _DataGridViewEx = value; }
        }

        public string RowHeaderText
        {
            get { return this._LeftView.Items[0].SubItems[0].Text; }
            set
            {
                this._LeftView.Items[0].SubItems[0].Text = value;
                this._RightView.Items[0].SubItems[0].Text = value;
            }
        }
        public int RowHeaderWidth
        {
            get { return this._LeftView.Columns[0].Width; }
            set
            {
                this._LeftView.Columns[0].Width = value;
                this._RightView.Columns[0].Width = value;
            }
        }


        public DataGridFooter(){
            this._ImgList.ImageSize = new Size(1, 18);
            this.Columns = new DataGridFooterColumnCollection(this);

            this.BorderStyle = BorderStyle.None;
            this._LeftPanel.BorderStyle = BorderStyle.None;
            this._RightPanel.BorderStyle = BorderStyle.None;
            this._LeftView.BorderStyle = BorderStyle.None;
            this._RightView.BorderStyle = BorderStyle.None;

            this.SuspendLayout();

            this.Height = 18;
            //
            //_LeftPanel
            //
            this._LeftPanel.Margin = new Padding(0);
            this._LeftPanel.Height = 18;

            //
            //_RightPanel
            //
            this._RightPanel.Margin = new Padding(0);
            this._RightPanel.Height = 18;

            //
            //_LeftView
            //
            this._LeftView.Margin = new Padding(0);
            this._LeftView.Height = 18;
            this._LeftView.Columns.Add("");
            this._LeftView.Items.Add("合计");

            this._LeftView.AutoArrange = false;
            this._LeftView.BackColor = System.Drawing.SystemColors.ActiveCaptionText;

            this._LeftView.BackgroundImage = global::GridFoot.Properties.Resources.bg;
            this._LeftView.BackgroundImageTiled = true;
            this._LeftView.BorderStyle = System.Windows.Forms.BorderStyle.None;

            this._LeftView.GridLines = true;
            this._LeftView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            
            this._LeftView.LabelWrap = false;
            this._LeftView.Name = "leftView";
            this._LeftView.Scrollable = false;
            this._LeftView.ShowGroups = false;
            this._LeftView.SmallImageList = this._ImgList;
            this._LeftView.UseCompatibleStateImageBehavior = false;
            this._LeftView.View = System.Windows.Forms.View.Details;

            //
            //_RightView
            //
            this._RightView.Margin = new Padding(0);
            this._RightView.Height = 18;
            this._RightView.Columns.Add("");
            this._RightView.Items.Add("合计");
            this._RightView.AutoArrange = false;
            this._RightView.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this._RightView.BackgroundImage = global::GridFoot.Properties.Resources.bg;
            this._RightView.BackgroundImageTiled = true;
            this._RightView.BorderStyle = System.Windows.Forms.BorderStyle.None;

            this._RightView.GridLines = true;
            this._RightView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;

            this._RightView.LabelWrap = false;
            this._RightView.Name = "rightView";
            this._RightView.Scrollable = false;
            this._RightView.ShowGroups = false;
            this._RightView.SmallImageList = this._ImgList;
            this._RightView.UseCompatibleStateImageBehavior = false;
            this._RightView.View = System.Windows.Forms.View.Details;
     

            this._LeftPanel.Controls.Add(this._LeftView);
            this._RightPanel.Controls.Add(this._RightView);

            this.Controls.Add(_LeftPanel);
            this.Controls.Add(_RightPanel);

            this.ResumeLayout(false);
            this.ParentChanged += new EventHandler(DataGridFooter_ParentChanged);
        }

        void DataGridFooter_ParentChanged(object sender, EventArgs e)
        {
            if (this.Parent != null && !(this.Parent is DataGridViewEx))
                throw new Exception("不能将DataGridView添加到非DataGridViewEx控件之中");
            _DataGridViewEx = (DataGridViewEx)this.Parent;
        }

        internal void ScrollWithParent(int scrollValue)
        {
            int leftw = _DataGridViewEx.RowHeadersVisible ? _DataGridViewEx.RowHeadersWidth : 0;
            if (_Frozen > 0)
            {
                for (int i = 0; i < _Frozen; i++)
                    leftw += _DataGridViewEx.Columns[i].Width;
            }
            _RightView.Left = -leftw-scrollValue+1;
        }

        public void LayoutControl(Rectangle rect)
        {
            //冻结列
            _Frozen = 0;
            this.RowHeaderWidth = this._DataGridViewEx.RowHeadersWidth-3;
            int sumw = 0;
            for(int i=0;i<this.Columns.Count&&i<this._DataGridViewEx.Columns.Count;i++)
            {
                this.Columns[i].Width=this._DataGridViewEx.Columns[i].Width;
                sumw += this.Columns[i].Width;
            }
            sumw += RowHeaderWidth;

            this._LeftView.Width = this._RightView.Width = sumw;
            
            for (int i = _DataGridViewEx.Columns.Count - 1; i >= 0; i--)
            {
                if (_DataGridViewEx.Columns[i].Frozen)
                {
                    _Frozen = ++i;
                    break;
                }
            }

            int leftw = _DataGridViewEx.RowHeadersVisible ? _DataGridViewEx.RowHeadersWidth : 0;
            if (_Frozen > 0)
            {
                for (int i = 0; i < _Frozen; i++)
                    leftw += _DataGridViewEx.Columns[i].Width;
            }
            _LeftPanel.Width = leftw;
            _LeftPanel.Left =_LeftView.Left = rect.Left;

            _RightPanel.Left = leftw + rect.Left;
            _RightPanel.Width = sumw;// rect.Width - leftw;

            _RightView.Left = -(_DataGridViewEx.RowHeadersVisible ? _DataGridViewEx.RowHeadersWidth : 0)+1;

            this.Width = rect.Width;
            this.Top = rect.Top;
            this.Left = rect.Left;

        }
    }
    public enum DataGridFooterSumMode{Sum,Average,Count,None}

    public class DataGridFooterColumn
    {
        private int _Width = 20;
        private object _Value;
        private DataGridFooterSumMode _SumMode = DataGridFooterSumMode.None;
        public string FormatStr;
        private DataGridFooter _DGFooter;
        internal int _ColumnIndex = -1;
        public string Text
        {
            get { return ""+_DGFooter._LeftView.Items[0].SubItems[_ColumnIndex].Text; }
            set { 
                _DGFooter._LeftView.Items[0].SubItems[_ColumnIndex].Text = value;
                _DGFooter._RightView.Items[0].SubItems[_ColumnIndex].Text = value;
            }
        }

        public object Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                Text = "" + value;
            }
        }

        public DataGridFooterSumMode SumMode
        {
            get { return _SumMode; }
            set { 
                _SumMode = value;
                if (_SumMode == DataGridFooterSumMode.Sum||_SumMode==DataGridFooterSumMode.Count||_SumMode==DataGridFooterSumMode.Average)
                    _Value = (decimal)0;
            }
        }

        public DataGridFooter DGFooter
        {
            get { return _DGFooter; }
            set { _DGFooter = value; }
        }

        public int Width
        {
            get { return _Width; }
            set
            {
                _Width = value;
                if (_ColumnIndex >= 0)
                {
                    this._DGFooter._LeftView.Columns[_ColumnIndex].Width = value;
                    this._DGFooter._RightView.Columns[_ColumnIndex].Width = value;
                }
            }
        }

        public DataGridFooterColumn()
        {
        
        }

        public DataGridFooterColumn(DataGridFooterSumMode summode)
        {
            this.SumMode = summode;
        }

    }

    public class DataGridFooterColumnCollection:CollectionBase
    {
        private DataGridFooter _DGFooter = null;
        public DataGridFooter DGFooter
        {
            get { return _DGFooter; }
            set { _DGFooter = value;
            for (int i = this.Count - 1; i >= 0; i--)
                this[i].DGFooter = value;
            }
        }
        public DataGridFooterColumn this[int index]{
            get
            {
                if (index >= 0 && index < List.Count)
                    return (DataGridFooterColumn)List[index];
                else
                    return null;
            }
        }
        
        public DataGridFooterColumnCollection()
        {

        }

        public DataGridFooterColumnCollection(DataGridFooter footer)
        {
            _DGFooter = footer;
        }
        internal void Add(DataGridFooterColumn column)
        {
            column.DGFooter = this.DGFooter;
           

            List.Add(column);
            column._ColumnIndex = List.Count;
            _DGFooter._LeftView.Columns.Add("", column.Width);
            _DGFooter._LeftView.Items[0].SubItems.Add("");
            _DGFooter._RightView.Columns.Add("", column.Width);
            _DGFooter._RightView.Items[0].SubItems.Add("");
        }
    }
}