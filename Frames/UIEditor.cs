using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using PythonAndTextEditor.Frames.FileWorks;

namespace PythonAndTextEditor.Frames
{
    public partial class UIEditor : Form
    {
        private string projectPath;

        public UIEditor()
        {
            InitializeComponent();
            projectPath = string.Empty;
        }

        private bool CheckProjectExists(bool isNeedShowMessage = true)
        {
            if (string.IsNullOrWhiteSpace(this.projectPath))
            {
                if (isNeedShowMessage)
                {
                    switch (MessageBox.Show(
                      "Необходимо сначала выбрать или создать проект. Создать новый проект?",
                      new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name,
                      MessageBoxButtons.YesNo))
                    {
                        case DialogResult.No:
                            break;
                        case DialogResult.Yes:
                            NewProject();
                            break;
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }



        private void NewTab(string filePath = null)
        {
            tabs.TabPages.Add(new RichTabPage(filePath, this.projectPath, tabs.TabCount + 1));
            tabs.SelectedIndex = tabs.TabCount - 1;
        }
        private void CloseTab(int? tabIndex = null, bool isNeedSave = true)
        {
            tabIndex = tabIndex.GetValueOrDefault(tabs.SelectedIndex);
            if (tabs.TabCount - tabIndex > 0)
            {
                if (isNeedSave)
                {
                    switch (MessageBox.Show("Хотите сохранить перед закрытием?", "Saving", MessageBoxButtons.YesNoCancel))
                    {
                        case DialogResult.Cancel:
                            return;
                        case DialogResult.No:
                            break;
                        case DialogResult.Yes:
                            SaveFile(tabIndex.Value);
                            break;
                    }
                }
                tabs.TabPages.RemoveAt(tabIndex.Value);
            }
        }





        #region File
        private void NewFile()
        {
            if (CheckProjectExists(false))
            {
                NewTab();
            }
        }
        private void NewFile(object sender, EventArgs e)
        {
            CheckProjectExists();
            NewFile();
        }

        private void OpenFile()
        {
            if (CheckProjectExists(false))
            {
                string filePath = FileHelper.ChooseFile(this.projectPath);
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    NewTab(filePath);
                }
            }
        }

        private void OpenFile(object sender, EventArgs e)
        {
            CheckProjectExists();
            OpenFile();
        }

        private void CloseFile(object sender, EventArgs e)
        {
            if (CheckProjectExists())
            {
                CloseTab();
            }
        }

        private void SaveFile(int? tabIndex = null)
        {
            tabIndex = tabIndex.GetValueOrDefault(tabs.SelectedIndex);
            (tabs.TabPages[tabIndex.Value] as RichTabPage).Save();
        }

        private void SaveFile(object sender, EventArgs e)
        {
            if (CheckProjectExists())
            {
                SaveFile(tabs.SelectedIndex);
            }
        } 
        #endregion





        private void NewProject()
        {
            this.projectPath = FileHelper.CreateProject();
        }

        private void NewProject(object sender, EventArgs e)
        {
            NewProject();
            OpenChoosedProject();
        }

        private void OpenProject()
        {
            this.projectPath = FileHelper.OpenProject();
        }

        private void OpenChoosedProject()
        {
            if (!string.IsNullOrWhiteSpace(this.projectPath))
            {
                var elements = FileHelper.GetProjectElements(this.projectPath);
                foreach (var filePath in elements)
                {
                    NewTab(filePath);
                }
            }
        }

        private void OpenProject(object sender, EventArgs e)
        {
            OpenProject();
            OpenChoosedProject();
        }

        private void CloseProject(object sender, EventArgs e)
        {
            if (CheckProjectExists())
            {
                switch (MessageBox.Show("Хотите сохранить перед закрытием?", "Close Project", MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Cancel:
                        return;
                    case DialogResult.No:
                        break;
                    case DialogResult.Yes:
                        SaveProject();
                        break;
                }
                while (tabs.TabCount > 0)
                {
                    CloseTab(0, false);
                }
                this.projectPath = string.Empty;
            }
        }

        private void SaveProject()
        {

            for (int i = 0; i < tabs.TabCount; i++)
            {
                SaveFile(i);
            }
        }
        private void SaveProject(object sender, EventArgs e)
        {
            if (CheckProjectExists())
            {
                SaveProject();
            }
        }
    }
}
