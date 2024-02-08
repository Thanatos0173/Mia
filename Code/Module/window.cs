using System.Threading.Tasks;
using System.Windows.Forms;

namespace myform
{
    class Program
    {

        static async Task Main()
        {
            await RunMyFormAsync();
        }
        public static async Task RunMyFormAsync()
        {
            Form myform = new Form();
            Button mybutton = new Button()
            {
                Text = "Hello",
                Location = new System.Drawing.Point(10, 10)
            };
            mybutton.Click += async (o, s) =>
            {
                await Task.Run(() =>
                {
                    MessageBox.Show("world");
                });
            };

            myform.Controls.Add(mybutton);
            myform.ShowDialog();

            while (myform.Created)
            {
                await Task.Delay(100);
            }
        }
    }
}