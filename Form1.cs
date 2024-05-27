using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using System.Security.Principal;

namespace EDP_ACTIVITY8_API
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient();
        public Form1()
        {
            InitializeComponent();
        }
        private async void LoadDepartments()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost/edpAct8API/api.php?departments=true");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var departments = JsonConvert.DeserializeObject<List<Department>>(responseBody);

                comboBox1.DisplayMember = "Dname";
                comboBox1.ValueMember = "Dnumber";
                comboBox1.DataSource = departments;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDepartments();
        }
        private async void LoadSalary(int dnumber)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"http://localhost/edpAct8API/api.php?salary=true&dnumber={dnumber}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var salary = JsonConvert.DeserializeObject<Salary>(responseBody);

                textBox5.Text = salary.Totalsalary.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }


        private async void getBtn_Click(object sender, EventArgs e)
        {
            try
            {
                textBox1.Clear();
                HttpResponseMessage response = await client.GetAsync("http://localhost/edpAct8API/api.php");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                string[] records = responseBody.Split(new[] { '}' }, StringSplitOptions.RemoveEmptyEntries);
                string formattedResponse = string.Join("}" + Environment.NewLine + Environment.NewLine, records);
                textBox1.Text = formattedResponse;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private async void postBtn_Click(object sender, EventArgs e)
        {
            var selectedDepartment = comboBox1.SelectedItem as Department;
            if (selectedDepartment == null)
            {
                MessageBox.Show("Please select a department.");
                return;
            }

            var userData = new
            {
                username = textBox2.Text,
                pass = textBox3.Text,
                email = textBox4.Text,
                dnumber = selectedDepartment.Dnumber,
                full_name = textBox6.Text,
                phone_number = textBox7.Text,
                address = textBox8.Text
            };

            string json = JsonConvert.SerializeObject(userData);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync("http://localhost/edpAct8API/api.php", content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                textBox1.Text = responseBody;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null)
            {
                int dnumber = (int)comboBox1.SelectedValue;
                LoadSalary(dnumber);
            }
        }
        public class Account
        {
            public int Userid { get; set; }
            public string Username { get; set; }
            public string Pass { get; set; }
            public string Email { get; set; }
        }
        public class Department
        {
            public int Dnumber { get; set; }
            public string Dname { get; set; }
            public int Totalsalary { get; set; }
        }

        public class Salary
        {
            public int Totalsalary { get; set; }
        }
    }
}
