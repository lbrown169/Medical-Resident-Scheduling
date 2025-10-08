SETUP AND INSTALLATION

1. Download Docker Desktop using: https://www.docker.com/products/docker-desktop/
2. Clone the repository
3. Open up terminal or cmd and go to the the project folder 





4. You will see a file called locked_psycall_prod_dump.sql.enc, it is an encrypted file of the database, you would need to unlock it using: 

openssl enc -d -aes-256-cbc -pbkdf2 -in locked_psycall_prod_dump.sql.enc -out psycall_prod_dump.sql && echo "✅ Decrypted locked_psycall_prod_dump.sql.enc → psycall_prod_dump.sql"

For the password, I will pin it on discord





5. From there run: 	docker compose up --build		<-the build for the frontend, backend and db gets started

!!!For those of you on windows, your firewall may block some processes so you’re gonna have to take care of that!!!

!!!Also, the first time you build it, it could take a few minutes!!!




ACCESS POINTS
Go to http://localhost:3000/ to access the website
Go to http://localhost:5109/swagger to access all the API endpoints




COMMANDS:

docker compose up --build    <- Builds frontend, backend and db

docker ps  <- You can verify all the containers are running 




REBUILDS EVERYTHING FROM SCRATCH:
docker compose down -v
docker compose build --no-cache
docker compose up --build

You can access MYSQL manually: 
docker exec -it psycall-db mysql -u psycalluser -p     <-Password: psycallpass, name of db: psycalldb



Encryption:
openssl enc -aes-256-cbc -pbkdf2 -salt -in psycall_prod_dump.sql -out locked_psycall_prod_dump.sql.enc && echo "✅ Encrypted psycall_prod_dump.sql → locked_psycall_prod_dump.sql.enc"



Decryption:
openssl enc -d -aes-256-cbc -pbkdf2 -in locked_psycall_prod_dump.sql.enc -out psycall_prod_dump.sql && echo "✅ Decrypted locked_psycall_prod_dump.sql.enc → psycall_prod_dump.sql"

#Password is pinned discord gc


-------------------------------------------------------------------------------------------------------



PITCH
* Create a scheduling application for medical residents that auto-generates a call schedule:
* Residents schedule changes monthly
* Be able to enter schedule of residents in order for the application to know who to pick from
* Make Blackout days for days residents cannot be scheduled for call
* Medical residents are able to add their vacation days to the applications, that are than approved by chief residents prior to generating schedule
* Schedule can be reviewed prior to publishing
* Be able to have the ability to switch calls after the schedule has been made between residents
* Have an administrative side in order to:
* Add/remove residents
* Edit residents schedule manually if needed
* Keep a running total of amount worked for each reside

WHAT DOES OUR CALL SCHEDULE LOOK LIKE?
* Short calls:
  - M-F from 4:30pm-8pm
* Weekend Calls:
  - Saturday 24 hour call 8am-8am
  - Needs the ability for residents to split into two 12 hour shifts
  - Saturday 12 hour call 8am-8pm
  - Sunday 12 hour call 8am-8pm
WHO IS SCHEDULED FOR CALLS?
* Residents:
  - Post-graduate year 1 (PGY1)
  - Post-graduate year 2 (PGY2)
  - Post-graduate year 3 (PGY3)
* July and August
  - PGY1 need to complete three training calls with a PGY3
  - PGY1 need to complete one 24 hours Saturday training call with a PGY2
  - PGY1 need to complete one 12 hour Sunday training call with a PGY2
* September through June
  - PGY1 and PGY2 complete short calls and weekend call
* SPECIAL HOLIDAY CALLS
  - July 4th - 12 hour (PGY2)
  - Labor Day - 2 hour (PGY2)
  - Thanksgiving - 24 hour (PGY1)
  - Black Friday
      - 24 hour (PGY1)
      - 12 hour (PGY2)
  - Christmas Day - 24 hour (PGY1)
  - New Years Day - 24 hour (PGY1)
  - Memorial Day - 12 hour (PGY1)

BLACKOUT DAYS
* Night rotation (one month during PGY1, two weeks during PGY2)
* Day before and after night rotation
* Emergency Medicine rotation (one month)
* IM inpatient rotation (one month)

Codebase Structure

Project Layers
  frontend/
    (React)


  backend/
* 1. Models
    Located in: MedicalDemo.Data.Models

    Represents the database tables using C# classes (e.g., Admins, Residents, Rotations, etc.)

    Managed through Entity Framework Core

    Configured via MedicalContext.cs

* 2. Repositories
    Located in: Repositories/

    IMedicalRepository.cs – Interface that defines methods for accessing all major tables.

    MedicalDataRepository.cs – Implements the data access logic using EF Core (e.g., fetch all residents, update admins).

    Serves as a bridge between the database and higher-level application logic (controllers/services).

* 3. API Controllers
    Located in: Controllers/

    Handles HTTP requests from the React frontend

    Uses MedicalRepository to access data

    Example: AdminController exposes /api/admin/all to get all admins


Frontend (React) → API Controller → Repository → Database


* Please Note:
    Our project currently does not use parameterized queries, which poses potential security risks such as SQL injection. To ensure a safe applitcation, the next development team should update all database interactions to use parameterized queries. Fortunately, ASP.NET makes this transition pretty straightforward through libraries like Entity Framework and ADO.NET.
