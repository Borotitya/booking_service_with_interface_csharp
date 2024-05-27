using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace TripPlanner
{
    public interface IBookingService
    {
        void Book(string destination, string fromDate, string toDate);
        double CalculateCost(int days);
        string GetCategory();
    }

    public class HotelBookingService : IBookingService
    {
        public void Book(string destination, string fromDate, string toDate)
        {
            MessageBox.Show($"Отель забронирован для направления: {destination}. С: {fromDate}. По: {toDate}", "Бронирование отеля", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public double CalculateCost(int days) => 30000.0 * days;

        public string GetCategory() => "Отель";
    }

    public class FlightBookingService : IBookingService
    {
        public void Book(string destination, string fromDate, string toDate)
        {
            MessageBox.Show($"Авиабилет забронирован для направления: {destination}. С: {fromDate}. По: {toDate}", "Бронирование авиабилета", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public double CalculateCost(int days) => 9000.0;

        public string GetCategory() => "Авиабилет";
    }

    public class TourBookingService : IBookingService
    {
        public void Book(string destination, string fromDate, string toDate)
        {
            MessageBox.Show($"Тур забронирован для направления: {destination}. С: {fromDate}. По: {toDate}", "Бронирование тура", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public double CalculateCost(int days) => 5000.0 * days;

        public string GetCategory() => "Тур";
    }

    public class RestaurantBookingService : IBookingService
    {
        public void Book(string destination, string fromDate, string toDate)
        {
            MessageBox.Show($"Ресторан забронирован для направления: {destination}. С: {fromDate}. По: {toDate}", "Бронирование ресторана", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public double CalculateCost(int days) => 1800.0 * days;

        public string GetCategory() => "Ресторан";
    }

    public class CarRentalBookingService : IBookingService
    {
        public void Book(string destination, string fromDate, string toDate)
        {
            MessageBox.Show($"Автомобиль арендован для направления: {destination}. С: {fromDate}. По: {toDate}", "Аренда автомобиля", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public double CalculateCost(int days) => 2000.0 * days;

        public string GetCategory() => "Автомобиль";
    }

    public partial class MainForm : Form
    {
        private ComboBox cbCategory;
        private TextBox tbDestination;
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private Label lblTotalCost;
        private double totalCost = 0;
        private List<IBookingService> bookingServices = new List<IBookingService>();
        private List<(string Category, string Destination, string FromDate, string ToDate)> bookings = new List<(string, string, string, string)>();

        public MainForm()
        {
            InitializeComponent();
            InitializeBookingServices();
        }

        private void InitializeComponent()
        {
            this.cbCategory = new ComboBox();
            this.tbDestination = new TextBox();
            this.dtpFrom = new DateTimePicker();
            this.dtpTo = new DateTimePicker();
            this.lblTotalCost = new Label();

            Button btnBook = new Button { Text = "Забронировать", Location = new Point(10, 180), Size = new Size(100, 30) };
            btnBook.Click += new EventHandler(this.Book);

            Button btnShowTable = new Button { Text = "Показать таблицу", Location = new Point(120, 180), Size = new Size(100, 30) };
            btnShowTable.Click += new EventHandler(this.ShowTable);

            Button btnShowPrices = new Button { Text = "Показать цены", Location = new Point(230, 180), Size = new Size(100, 30) };
            btnShowPrices.Click += new EventHandler(this.ShowPrices);

            this.cbCategory.Location = new Point(10, 140);
            this.cbCategory.Size = new Size(200, 30);
            this.cbCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            this.tbDestination.Location = new Point(10, 30);
            this.tbDestination.Size = new Size(280, 30);

            this.dtpFrom.Location = new Point(10, 90);
            this.dtpFrom.Size = new Size(200, 30);

            this.dtpTo.Location = new Point(220, 90);
            this.dtpTo.Size = new Size(200, 30);

            this.lblTotalCost.Location = new Point(10, 220);
            this.lblTotalCost.Size = new Size(200, 30);
            this.lblTotalCost.Text = "Общая стоимость: 0.00 руб.";

            Label lblCategory = new Label { Text = "Выберите категорию:", Location = new Point(10, 120), Size = new Size(200, 20) };
            Label lblDestination = new Label { Text = "Введите город для отдыха:", Location = new Point(10, 10), Size = new Size(200, 20) };
            Label lblFromDate = new Label { Text = "Выбор даты от:", Location = new Point(10, 70), Size = new Size(200, 20) };
            Label lblToDate = new Label { Text = "Выбор даты до:", Location = new Point(220, 70), Size = new Size(200, 20) };

            this.Controls.Add(lblCategory);
            this.Controls.Add(lblDestination);
            this.Controls.Add(lblFromDate);
            this.Controls.Add(lblToDate);
            this.Controls.Add(this.cbCategory);
            this.Controls.Add(this.tbDestination);
            this.Controls.Add(this.dtpFrom);
            this.Controls.Add(this.dtpTo);
            this.Controls.Add(this.lblTotalCost);
            this.Controls.Add(btnBook);
            this.Controls.Add(btnShowTable);
            this.Controls.Add(btnShowPrices);

            this.Text = "Планировщик поездок";
            this.Size = new Size(450, 300);
        }

        private void InitializeBookingServices()
        {
            bookingServices.Add(new HotelBookingService());
            bookingServices.Add(new FlightBookingService());
            bookingServices.Add(new TourBookingService());
            bookingServices.Add(new RestaurantBookingService());
            bookingServices.Add(new CarRentalBookingService());

            foreach (var service in bookingServices)
            {
                cbCategory.Items.Add(service.GetCategory());
            }
        }

        private void Book(object sender, EventArgs e)
        {
            string destination = tbDestination.Text;
            string fromDate = dtpFrom.Value.ToString("dd/MM/yyyy");
            string toDate = dtpTo.Value.ToString("dd/MM/yyyy");

            if (cbCategory.SelectedIndex == -1)
            {
                MessageBox.Show("Пожалуйста, выберите категорию.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IBookingService service = bookingServices[cbCategory.SelectedIndex];
            int days = (dtpTo.Value - dtpFrom.Value).Days + 1;

            double cost = service.CalculateCost(days);
            totalCost += cost;
            service.Book(destination, fromDate, toDate);

            bookings.Add((service.GetCategory(), destination, fromDate, toDate));

            lblTotalCost.Text = $"Общая стоимость: {totalCost:0.00} руб.";
        }

        private void ShowTable(object sender, EventArgs e)
        {
            Form tableForm = new Form { Text = "Таблица бронирований", Size = new Size(500, 300) };

            TextBox tbBookings = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Dock = DockStyle.Fill };
            tableForm.Controls.Add(tbBookings);

            foreach (var booking in bookings)
            {
                tbBookings.AppendText($"{booking.Category}: {booking.Destination} с {booking.FromDate} по {booking.ToDate}{Environment.NewLine}");
            }

            tableForm.Show();
        }

        private void ShowPrices(object sender, EventArgs e)
        {
            Form pricesForm = new Form { Text = "Список цен", Size = new Size(300, 200) };

            TextBox tbPrices = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Dock = DockStyle.Fill };
            pricesForm.Controls.Add(tbPrices);

            foreach (var service in bookingServices)
            {
                tbPrices.AppendText($"{service.GetCategory()}: {service.CalculateCost(1)} руб. в день{Environment.NewLine}");
            }

            pricesForm.Show();
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
