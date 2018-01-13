using Encog.Persist;
using System;
using System.IO;
using System.Windows.Forms;

namespace enc.Utils
{
    static class WinPersistence
    {
        public static object LoadSaved(string filename)
        {
            if (!String.IsNullOrEmpty(filename))
            {
                return EncogDirectoryPersistence.LoadObject(new FileInfo(filename));
            }
            else
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "Encog Files (*.EG)|*.EG",
                    DefaultExt = "EG",
                    AddExtension = true
                };
                dialog.ShowDialog();
                filename = dialog.FileName;

                return EncogDirectoryPersistence.LoadObject(new FileInfo(filename));
            }
        }

        public static bool Save(object obj, string filename)
        {
            if (!String.IsNullOrEmpty(filename))
            {
                EncogDirectoryPersistence.SaveObject(new FileInfo(filename), obj);
                return true;
            }
            else
            {
                var dialog = new SaveFileDialog
                {
                    Filter = "Encog Files (*.EG)|*.EG",
                    DefaultExt = "EG",
                    AddExtension = true
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    filename = dialog.FileName;
                    EncogDirectoryPersistence.SaveObject(new FileInfo(filename), obj);
                    return true;
                }
            }
            return false;
        }
    }
}
