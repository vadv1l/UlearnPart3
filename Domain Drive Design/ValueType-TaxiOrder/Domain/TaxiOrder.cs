using System;
using System.Globalization;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using Ddd.Infrastructure;
using System.Xml.Linq;
using NUnit.Framework;

namespace Ddd.Taxi.Domain
{
	// In real aplication it whould be the place where database is used to find driver by its Id.
	// But in this exercise it is just a mock to simulate database
	public class DriversRepository
	{
		public Driver GetDriverById(int driverId)
        {
            if (driverId == 15)
                return new Driver(15, new PersonName("Drive", "Driverson"), new Car("Baklazhan",
                    "Lada sedan", "A123BT 66"));
            throw new Exception("Unknown driver id " + driverId);
        }
	}

	public class TaxiApi : ITaxiApi<TaxiOrder>
	{
		private readonly DriversRepository driversRepo;
		private readonly Func<DateTime> currentTime;
		private int idCounter;

		public TaxiApi(DriversRepository driversRepo, Func<DateTime> currentTime)
		{
			this.driversRepo = driversRepo;
			this.currentTime = currentTime;
		}

        public TaxiOrder CreateOrderWithoutDestination(string firstName, string lastName, string street,
            string building) => TaxiOrder.CreateOrderWithoutDestination(idCounter++, 
            firstName, lastName, street, building, currentTime);

        public void UpdateDestination(TaxiOrder order, string street, string building) =>
            order.UpdateDestination(new Address(street, building));

        public void AssignDriver(TaxiOrder order, int driverId) =>
            order.AssignDriver(driversRepo.GetDriverById(driverId), currentTime);

        public void UnassignDriver(TaxiOrder order) =>
            order.UnassignDriver();

        public string GetDriverFullInfo(TaxiOrder order) =>
            order.GetDriverFullInfo();

        public string GetShortOrderInfo(TaxiOrder order) => order.GetShortOrderInfo();
        public void Cancel(TaxiOrder order) => order.Cancel(currentTime);
        public void StartRide(TaxiOrder order) => order.StartRide(currentTime);
        public void FinishRide(TaxiOrder order) => order.FinishRide(currentTime);
    }

	public class TaxiOrder : Entity<int>
    {
        public PersonName ClientName { get; private set; }
        public Address Start { get; private set; }
        public Address Destination { get; private set; }
        public Driver Driver { get; private set; }
        private TaxiOrderStatus Status;
        private DateTime CreationTime;
        private DateTime DriverAssignmentTime;
        private DateTime CancelTime;
        private DateTime StartRideTime;
        private DateTime FinishRideTime;

        public TaxiOrder(int id) : base(id) { }

        public static TaxiOrder CreateOrderWithoutDestination(int id, 
            string firstName, string lastName, string street, string building,
            Func<DateTime> currentTime)
        {
            return
                new TaxiOrder(id)
                {
                    ClientName = new PersonName(firstName, lastName),
					Start = new Address(street, building),
                    CreationTime = currentTime()
                };
        }

        public void UpdateDestination(Address address)
        {
            Destination = address;
        }

        public void AssignDriver(Driver driver, Func<DateTime> currentTime)
        {
            if (Driver != null) throw new InvalidOperationException(nameof(TaxiOrderStatus.WaitingCarArrival));
            Driver = driver;
            DriverAssignmentTime = currentTime();
            Status = TaxiOrderStatus.WaitingCarArrival;
        }

        public void UnassignDriver()
        {
            if (Driver == null || Status == TaxiOrderStatus.InProgress) 
                throw new InvalidOperationException(nameof(TaxiOrderStatus.WaitingForDriver));
            Driver = null;
            Status = TaxiOrderStatus.WaitingForDriver;
        }

        public string GetDriverFullInfo()
        {
            if (Status == TaxiOrderStatus.WaitingForDriver) return null;
            return string.Join(" ",
                "Id: " + Driver.Id,
                "DriverName: " + ((Driver == null) ? "" : FormatName(Driver.DriverName)),
                "Color: " + Driver.Car.CarColor,
                "CarModel: " + Driver.Car.CarModel,
                "PlateNumber: " + Driver.Car.CarPlateNumber);
        }

        public string GetShortOrderInfo()
        {
            return string.Join(" ",
                "OrderId: " + Id,
                "Status: " + Status,
                "Client: " + FormatName(ClientName),
                "Driver: " + ((Driver == null) ? "" : FormatName(Driver.DriverName)),
                "From: " + FormatAddress(Start),
                "To: " + FormatAddress(Destination),
                "LastProgressTime: " + GetLastProgressTime()
                    .ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        }

        private DateTime GetLastProgressTime()
        {
            if (Status == TaxiOrderStatus.WaitingForDriver) return CreationTime;
            if (Status == TaxiOrderStatus.WaitingCarArrival) return DriverAssignmentTime;
            if (Status == TaxiOrderStatus.InProgress) return StartRideTime;
            if (Status == TaxiOrderStatus.Finished) return FinishRideTime;
            if (Status == TaxiOrderStatus.Canceled) return CancelTime;
            throw new NotSupportedException(Status.ToString());
        }

        private string FormatName(PersonName name)
        {
            return string.Join(" ", new[] { name.FirstName, name.LastName }.Where(n => n != null));
        }

        private string FormatAddress(Address address)
        {
            if (address == null) return "";
            return string.Join(" ", new[] { address.Street, address.Building }.Where(n => n != null));
        }

        public void Cancel(Func<DateTime> currentTime)
        {
            if (Status == TaxiOrderStatus.InProgress)
                throw new InvalidOperationException(nameof(TaxiOrderStatus.InProgress));
            Status = TaxiOrderStatus.Canceled;
            CancelTime = currentTime();
        }

        public void StartRide(Func<DateTime> currentTime)
        {
            if (Status == TaxiOrderStatus.WaitingForDriver)
                throw new InvalidOperationException(nameof(TaxiOrderStatus.WaitingForDriver));
            Status = TaxiOrderStatus.InProgress;
            StartRideTime = currentTime();
        }

        public void FinishRide(Func<DateTime> currentTime)
        {
            if (Status != TaxiOrderStatus.InProgress)
                throw new InvalidOperationException(nameof(TaxiOrderStatus.InProgress));
            Status = TaxiOrderStatus.Finished;
            FinishRideTime = currentTime();
        }
    }

    public class Driver : Entity<int>
    {
        public Driver(int id, PersonName name, Car car) : base(id)
        {
            DriverName = name;
            Car = car;
        }

		public Car Car { get; }
		public PersonName DriverName { get; }
    }

    public class Car : ValueType<Car>
    {
        public Car(string carColor, string carModel, string carPlateNumber)
        {
            CarColor = carColor;
			CarModel = carModel;
            CarPlateNumber = carPlateNumber;
        }

        public string CarColor { get; }
        public string CarModel { get; }
        public string CarPlateNumber { get; }
    }
}