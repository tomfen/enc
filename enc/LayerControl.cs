using System;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using Encog.Engine.Network.Activation;
using Encog.Neural.Networks.Layers;

namespace enc
{
    public partial class LayerControl : UserControl
    {
        public enum LayerType
        {
            Input,
            Output,
            Hidden,
        }

        public LayerControl(LayerType type, int? neurons)
        {
            InitializeComponent();

            Assembly assembly = Assembly.GetAssembly(typeof(IActivationFunction));

            var functions = from t in assembly.GetTypes()
                            where t.GetInterfaces().Contains(typeof(IActivationFunction))
                            select t;

            var data = functions.Select(t => new {
                Name = t.Name,
                Value = Activator.CreateInstance(t),
            }).ToList();

            comboBox1.DataSource = data;
            comboBox1.ValueMember = "Value";
            comboBox1.DisplayMember = "Name";

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            if (type == LayerType.Input)
            {
                comboBox1.SelectedIndex = comboBox1.FindStringExact("ActivationLinear");
                comboBox1.Enabled = false;
                numericUpDown1.Value = (decimal)neurons;
                numericUpDown1.Enabled = false;
            }
            else if(type == LayerType.Output)
            {
                numericUpDown1.Enabled = false;
                numericUpDown1.Value = (decimal)neurons;
                checkBox1.Enabled = false;
                checkBox1.Checked = false;
            }
        }

        internal ILayer CreateLayer()
        {
            var activation = (IActivationFunction)comboBox1.SelectedValue;
            return new BasicLayer(activation, checkBox1.Checked, (int)numericUpDown1.Value);
        }
    }
}
