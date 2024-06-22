## FintaCharts

### How to Run

1. **Clone the Repository:**

   ```bash
   git clone https://github.com/yourusername/yourproject.git
   cd yourproject

2. **Run Docker Compose:**

  ```bash
  Copy code
  docker-compose up
  This command starts the application and sets up the necessary environment.

3. **Access the Application:**

The application will start and listen on http://localhost:8080.
Database File:

Docker Compose will create an app.db file for data storage. SQLite is used for simplicity.
Environment:

The application environment is set to development.
4. **Swagger Documentation:**

Access Swagger UI for API documentation: http://localhost:8080/swagger

5. **API Endpoints:**
You can get the last price/bid/ask either with ID or symbol
Example
GET FintaCharts/GetInstrumentLastBySymbol?symbol=AUD%2FCAD: Retrieve last prices for an instrument.
