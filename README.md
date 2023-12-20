# Appointment Reservation System

This is a appointment reservation backend system designed for providers and clients.

## Task
Build an API (e.g. RESTful) with the following endpoints:
- Allows providers to submit times they are available for appointments 
	- e.g. On Friday the 13th of August, Dr. Jekyll wants to work between 8am and 3pm
- Allows a client to retrieve a list of available appointment slots 
- Appointment slots are 15 minutes long
- Allows clients to reserve an available appointment slot
- Allows clients to confirm their reservation
- Reservations expire after 30 minutes if not confirmed and are again available for other clients to reserve that appointment slot
- Reservations must be made at least 24 hours in advance


## Prerequisites

Before you begin, ensure you have met the following requirements:

* You have installed the latest version of [.NET 8.0](https://dotnet.microsoft.com/download)
* You have read [the official .NET documentation](https://docs.microsoft.com/en-us/dotnet/).
* You are using a version of Visual Studio 2022 (Commercial/Professional or Enterprise).
* You have installed the [Postman client](https://www.postman.com/downloads/) for testing APIs.

## Installing Appointment Reservation System

To install Appointment Reservation System, follow these steps:

1. Clone the repository: `git clone https://github.com/madhav5589/Reservation.git`
2. Open the project in Visual Studio.
3. Restore the NuGet packages: `dotnet restore`
4. Build the project: `dotnet build`

## Using Appointment Reservation System

To use Appointment Reservation System, follow these steps:

1. Run the project: `dotnet run` from CLI. If you prefer to use Visual Studio, you can click on Debug option from the top menu and click on Start/Start without Debugging. You can also use keyboard shortcut - F5 or Ctrl + F5.
2. Open Postman client and execute different API endpoints by sending requests. For example,
	- To submit provider's availability, send `HttpPost` request to this endpoint: `http://localhost:5199/api/appointment/submit`. Provide the following sample data in the Body section in the JSON format:
		```json
		{
			"ProviderId": 1,
			"StartTime": "2023-12-20T10:00:00",
			"EndTime": "2023-12-22T12:00:00",
			"IsReserved": false
		}
		```

	- In order to get all available appointments by provider id, send `HttpGet` request to this endpoint: `http://localhost:5199/api/appointment/1/available`. Here, we have provided ProviderId = 1.
	- In order to reserve an appointment, send `HttpPost` request to this endpoint: `http://localhost:5199/api/appointment/reserve/10`. Here we have provided AppointmentId = 10.
	- In order to confirm the appointment after reservation, send `HttpPost` request to this endpoint: `http://localhost:5199/api/appointment/confirm/10`. Here we have provided AppointmentId = 10.



## Contributing to Appointment Reservation System

To contribute to Appointment Reservation System, follow these steps:

1. Fork this repository.
2. Create a branch: `git checkout -b <branch_name>`.
3. Make your changes and commit them: `git commit -m '<commit_message>'`
4. Push to the original branch: `git push origin <project_name>/<location>`
5. Create the pull request.
