﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VectorChimera
{
    class ColorLists
    {
        private ListBox paletteBoxOld;
        private ListBox paletteBoxNew;
        private MainForm Main;

        public Action<Dictionary<int,int>> RefreshImage;
        public Func<Color,int,ColorResult> ShowColorDialog;

        public ColorLists(System.Windows.Forms.ListBox paletteBoxOld, System.Windows.Forms.ListBox paletteBoxNew, MainForm main)
        {
            this.paletteBoxOld = paletteBoxOld;
            this.paletteBoxNew = paletteBoxNew;
            this.Main = main;

            paletteBoxOld.DrawItem += paletteBoxOld_DrawItem;
            paletteBoxNew.DrawItem += paletteBoxNew_DrawItem;

            paletteBoxOld.MouseDoubleClick += paletteOldBox_MouseDoubleClick;
            paletteBoxNew.MouseDoubleClick += paletteNewBox_MouseDoubleClick;

            paletteBoxNew.SelectedIndexChanged += paletteBoxNew_SelectedIndexChanged;
            paletteBoxOld.SelectedIndexChanged += paletteBoxOld_SelectedIndexChanged;
        }

        private void paletteBoxNew_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawListColor(paletteBoxNew, e);
        }
        private void paletteBoxOld_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawListColor(paletteBoxOld, e);
        }

        private void DrawListColor(ListBox box, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index < 0) return;

            int item = (int)box.Items[e.Index]; // Get the current item and cast it to MyListBoxItem

            int border = 5;
            int lineBorder = 2;

            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle
                (e.Bounds.X + border, e.Bounds.Y + border,
                e.Bounds.Width - border * 2, e.Bounds.Height - border * 2));
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(item)), new Rectangle
                            (e.Bounds.X + border + lineBorder, e.Bounds.Y + border + lineBorder,
                            e.Bounds.Width - border * 2 - lineBorder * 2, e.Bounds.Height - border * 2 - lineBorder * 2));

            e.DrawFocusRectangle();
        }

        
        void paletteBoxOld_SelectedIndexChanged(object sender, EventArgs e)
        {
            paletteBoxNew.SelectedIndex = paletteBoxOld.SelectedIndex;
            paletteBoxNew.TopIndex = paletteBoxOld.TopIndex;
        }

        void paletteBoxNew_SelectedIndexChanged(object sender, EventArgs e)
        {
            paletteBoxOld.SelectedIndex = paletteBoxNew.SelectedIndex;
            paletteBoxOld.TopIndex = paletteBoxNew.TopIndex;
        }

        void paletteOldBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (paletteBoxOld.Items.Count>0) ChooseColor(Color.FromArgb((int)paletteBoxOld.SelectedItem),(int)paletteBoxOld.SelectedItem);
        }

        void paletteNewBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (paletteBoxNew.Items.Count > 0) ChooseColor(Color.FromArgb((int)paletteBoxNew.SelectedItem), (int)paletteBoxOld.Items[paletteBoxNew.SelectedIndex]);
        }

        private void ChooseColor(Color from, int index)
        {
            if (ShowColorDialog == null) return;
            ColorResult colorResult = ShowColorDialog(from, index);

            if (colorResult.Result == DialogResult.OK)
            {
                ReplaceColor(index, colorResult.Color,false);
            }
            else
            {
                ReplaceColor(index, from, false);
            }
        }

        public void ReplaceColor(int index, Color colorResult, bool preview)
        {
            if (preview)
            {
                Dictionary<int, int> previewChangelog = new Dictionary<int, int>(Main.Palette);
                previewChangelog[index] = colorResult.ToArgb();
                RefreshImage(previewChangelog);
            }
            else
            {
                Main.Palette[index] = colorResult.ToArgb();
                RefreshPalette();
                RefreshImage(Main.Palette);
            }
        }

        public void RefreshPalette()
        {
            paletteBoxOld.Items.Clear();
            foreach (var c in Main.Palette.Keys) paletteBoxOld.Items.Add(c);
            paletteBoxNew.Items.Clear();
            foreach (var c in Main.Palette.Values) paletteBoxNew.Items.Add(c);
        }

        internal void SelectColor(Color selectedColor, bool old=false)
        {
            for (int i =0; i<paletteBoxNew.Items.Count;i++)
            {
                if ((int)paletteBoxNew.Items[i] == selectedColor.ToArgb())
                {
                    SelectColor(i);
                }
            }
        }

        private void SelectColor(int i)
        {
            paletteBoxNew.SelectedIndex = i;
            paletteBoxOld.SelectedIndex = i;
        }
    }
}
