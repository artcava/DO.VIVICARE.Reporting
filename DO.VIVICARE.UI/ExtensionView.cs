using System.Windows.Forms;

namespace DO.VIVICARE.UI
{
    public static class ExtensionView
    {
        public static void MountHeaders(this ListView lv, params object[] i)
        {
            lv.Columns.Clear();
            for (int j = i.GetLowerBound(0); j <= i.GetUpperBound(0); j += 3)
            {
                lv.Columns.Add((string)i[j], (int)i[j + 1], (HorizontalAlignment)i[j + 2]);
            }
        }

        public static void AddRow(this ListView lv, int obj, string title, params string[] subtitle)
        {
            ListViewItem n = lv.Items.Add(title, 1);
            n.ImageIndex = obj;
            for (int i = subtitle.GetLowerBound(0); i <= subtitle.GetUpperBound(0); i++)
            {
                n.SubItems.Add(subtitle[i]);
            }
        }
    }
}
