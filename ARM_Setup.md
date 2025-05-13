# Instructions for setting up ARM based DVTA Instance

#### Key Differences:
| x64/x86       | ARM |
|------------|-----|
| Express 2008 | SQLite  |
| Dotnet 4.5 | Dotnet 4.9+ |
| Config Server button to set DB | Assumes DB is SQLite file (in repo)  |
| Filezilla FTP server    | Windows IIS FTP client localhost |

### Instructions for setup specific to ARM

#### Windows FTP
1. Search add features > select IIS & FTP & IIS management console
2. pen IIS management console > sites > add FTP site
3. Set directory as public/Documents. Port 21. No SSL
4. Create a new user "dvta" and set password as "p@ssw0rd" (user gui, follow your nose)
5. Firewall rules > enable `Microsoft FTP Server (FTP Traffic In)`

### Notes from modifying the DB to use SQLite
1. Installed System.Data.SQLite
2. Modified SQL commands to suit SQLite
3. Modified queries slightly to support SQLite syntax
4. Updated App.Config to replace the SQL server connection to a new .sqlite file
5. Created `dvta.sqlite` with same users and expsnses as mentioned in original repo