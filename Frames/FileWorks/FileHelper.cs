using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PythonAndTextEditor.Frames.FileWorks
{
    public static class FileHelper
    {

        //private static string pythonExt = ".py";
        //private static string editorExt = ".mle";
        //private static string projectExt = ".pyte";

        private static string FilterExtention => 
            $"*{GetExtention(Extentions.python)} | *{GetExtention(Extentions.editor)}";

        private static string HintExtention =>
            $"{GetExtention(Extentions.python)} or {GetExtention(Extentions.editor)} file";

        private static string ProjectSettingsName =>
            $"setting{GetExtention(Extentions.project)}";


        public enum Extentions
        {
            python,
            editor,
            project
        }

        public static string GetExtention(Extentions ext)
        {
            string res = string.Empty;

            switch(ext)
            {
                case Extentions.python:
                    res = ".py";
                    break;
                case Extentions.editor:
                    res = ".mle";
                    break;
                case Extentions.project:
                    res = ".pyte";
                    break;
            }
            return res;
        }


        public static string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        public static string CreateProject()
        {
            string filePath = string.Empty;
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = Path.Combine(dialog.SelectedPath, ProjectSettingsName);
                    if (CheckFileExists(filePath))
                    {
                        switch (MessageBox.Show("В выбранной папке уже существует проект, использовать его?", "Create ProjectTabs", MessageBoxButtons.YesNoCancel))
                        {
                            case DialogResult.Cancel:
                                filePath = string.Empty;
                                break;
                            case DialogResult.No:
                                File.Delete(filePath);
                                break;
                            case DialogResult.Yes:
                                break;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(filePath) && !File.Exists(filePath))
                    {
                        File.Create(filePath);
                    }
                }
            }
            return filePath;
        }

        public static string OpenProject()
        {
            string filePath = string.Empty;
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = Path.Combine(dialog.SelectedPath, ProjectSettingsName);
                    if (!CheckFileExists(filePath))
                    {
                        filePath = string.Empty;
                        MessageBox.Show("В выбранном проекте нет файла настроек проекта", "Open ProjectTabs");
                    }
                }
            }
            return filePath;
        }

        public static bool CheckFileExists(string filePath)
        {
            return !string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath);
        }

        public static string OpenFile(string filePath)
        {
            List<string> code = new List<string>(); 
            string line;

            using (StreamReader reader = new StreamReader(filePath))
            {
                line = reader.ReadLine();
                while (!string.IsNullOrWhiteSpace(line))
                {
                    code.Add(line);
                    line = reader.ReadLine();
                }
            }

            return string.Join("\n", code);
        }


        /// <summary>
        /// Сохранение содержимого закладки в файл проекта
        /// </summary>
        /// <param name="tab"></param>
        public static void SaveFileToProject(RichTabPage tab)
        {
            if(!FileHelper.CheckFileExists(tab.filePath))
            {
                SaveFileDialog dialog = new SaveFileDialog()
                {
                    Filter = $"*{GetExtention(Extentions.python)}|*{GetExtention(Extentions.editor)}",
                    Title = $"Save {GetExtention(Extentions.python)} or {GetExtention(Extentions.editor)}",
                    InitialDirectory = tab.projectPath,
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    tab.filePath = dialog.FileName;
                }
            }

            using(StreamWriter writer = new StreamWriter(tab.filePath))
            {
                writer.WriteLine(tab.box.Text);
            }

            var elements = GetProjectElements(tab.projectPath);

            if(!elements.Contains(tab.filePath))
            {

                using (StreamWriter writer = new StreamWriter(tab.projectPath))
                {
                    writer.WriteLine(tab.filePath);
                }
            }
        }
        
        public static List<string> GetProjectElements(string projectPath)
        {
            List<string> elements = new List<string>();
            if (File.Exists(projectPath))
            {
                string settings = OpenFile(projectPath);
                if (!string.IsNullOrWhiteSpace(settings))
                {
                    elements = settings.Split('\n').ToList();
                }
            }
            return elements;
        }

        public static string ChooseFile(string folderPath, string filePath = null)
        {
            if (!CheckFileExists(filePath))
            {
                using (OpenFileDialog dialog = new OpenFileDialog()
                {
                    Filter = FilterExtention,
                    Title = $"Choose {HintExtention} to open it in project",
                    InitialDirectory = folderPath,
                    Multiselect = false,
                })
                {
                    filePath = dialog.ShowDialog() == DialogResult.OK ?
                        dialog.FileName : null;
                }
            }

            return filePath;
        }
    }
}
