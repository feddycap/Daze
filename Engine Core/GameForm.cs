﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Daze {
    public class GameForm:Form {
        #region Variables
        internal bool loaded = false;
        private PictureBox gameFrame;
        internal Bitmap buffer;
        internal bool focus = false;
        #endregion

        #region Methods
        internal GameForm() {
            //form
            DoubleBuffered = true;
            Text = "GameForm";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            //gameFrame
            this.gameFrame = new PictureBox();
            gameFrame.Dock = DockStyle.Fill;
            this.Controls.Add(this.gameFrame);

            //events
            FormClosed += new FormClosedEventHandler(this.GameForm_FormClosed);
            Shown += new EventHandler(this.GameForm_Shown);
            GotFocus += new EventHandler(this.Got_Focus);
            LostFocus += new EventHandler(this.Lost_Focus);

            //events passed to the Engine
            gameFrame.MouseClick += GameFrame_MouseClick;
            gameFrame.MouseDoubleClick += GameFrame_MouseDoubleClick;

            gameFrame.MouseMove += GameFrame_MouseMove;

            gameFrame.MouseDown += GameFrame_MouseDown;
            gameFrame.MouseUp += GameFrame_MouseUp;
        }

        #region Event passed to Engine
        private void GameFrame_MouseClick(object sender, MouseEventArgs e) { Engine.mouseClick?.Invoke(sender, e); }
        private void GameFrame_MouseDoubleClick(object sender, MouseEventArgs e) { Engine.mouseDoubleClick?.Invoke(sender, e); }

        private void GameFrame_MouseMove(object sender, MouseEventArgs e) { Engine.mouseMove?.Invoke(sender, e); }

        private void GameFrame_MouseDown(object sender, MouseEventArgs e) { Engine.mouseDown?.Invoke(sender, e); }
        private void GameFrame_MouseUp(object sender, MouseEventArgs e) { Engine.mouseUp?.Invoke(sender, e); }
        #endregion



        #region Event handlers
        private void GameForm_FormClosed(object sender, FormClosedEventArgs e) {
            Environment.Exit(0);
        }

        private void GameForm_Shown(object sender, EventArgs e) {
            loaded = true;
        }

        private void Lost_Focus(object sender, EventArgs e) {
            focus = false;
            Engine.lostFocus?.Invoke();
        }

        private void Got_Focus(object sender, EventArgs e) {
            focus = true;
            Engine.gotFocus?.Invoke();
        }
        #endregion

        #region Drawing methods
        internal void updateImage() {
            if(focus) {
                buffer.Dispose();
                buffer = new Bitmap(Engine._drawBufferWidth, Engine._drawBufferHeight, PixelFormat.Format24bppRgb);
                BitmapData bmpData = buffer.LockBits(new Rectangle(0, 0, buffer.Width, buffer.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                for(int y = 0; y < Engine._drawBufferHeight; y++) {
                    IntPtr startOfLine = bmpData.Scan0 + y * bmpData.Stride;
                    Marshal.Copy(Engine.drawBuffer, y * Engine.drawBufferStride, startOfLine, Engine.drawBufferStride);
                }

                buffer.UnlockBits(bmpData);

                gameFrame.Image = buffer;
            }
        }
        #endregion

        #endregion
    }
}
