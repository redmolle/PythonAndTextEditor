using PythonAndTextEditor.Frames.FileWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PythonAndTextEditor.Frames
{
    public class RichTabPage : TabPage
    {
        public RichTextBox box;
        public string filePath;
        public string projectPath;

        public RichTabPage(string fullFileName, string projectPath, int tabIndex)
        {
            box = new RichTextBox();
            this.Controls.Add(box);
            box.Dock = DockStyle.Fill;
            if (FileHelper.CheckFileExists(fullFileName))
            {
                this.filePath = fullFileName;
                box.Text = FileHelper.OpenFile(fullFileName);
                this.Text = FileHelper.GetFileName(fullFileName);
            }
            else
            {
                this.Text = $"Tab {tabIndex.ToString()}";
            }
            this.projectPath = projectPath;
        }

        public void Save()
        {
            FileHelper.SaveFileToProject(this);
        }
    }
}
