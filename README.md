# The Vault 🔫

A Fallout-themed terminal application for tracking ammunition inventory, built with C# and Supabase.

```ascii
___  ___         _   _                _  _   
|  \/  |        | | | |              | || |  
| .  . | _   _  | | | |  __ _  _   _ | || |_ 
| |\/| || | | | | | | | / _` || | | || || __|
| |  | || |_| | \ \_/ /| (_| || |_| || || |_ 
\_|  |_/ \__, |  \___/  \__,_| \__,_||_| \__|
          __/ |                              
         |___/   
```

## 📝 Description

The Vault is an ammunition tracking tool inspired by the Fallout series' Pip-Boy interface. It helps shooting enthusiasts maintain an accurate inventory of their ammunition through a retro-style terminal interface, complete with the iconic green text and animations that Fallout fans will recognize.

## ✨ Features
- Track multiple ammunition types with detailed information:
  - Caliber
  - Brand
  - Purchase price
  - Purchase date
  - Quantity
- Comprehensive logging system for ammunition transactions
- Fallout-inspired terminal interface with:
  - Custom ASCII art
  - Pip-Boy-style green text
  - Retro animations
- Persistent data storage using Supabase

## 🗺️ Roadmap

### 🎯 Core Features
- [ ] Create Operation
  - [ ] `feat: add create ammunition menu option`
  - [ ] `feat: implement create ammunition logic`
  - [ ] `feat: add input validation for new ammo entries`
  - [ ] `feat: add success/error messages for creation`

- [ ] Update Operation
  - [ ] `feat: add update ammunition menu option`
  - [ ] `feat: implement ammunition search by ID`
  - [ ] `feat: add update ammunition logic`
  - [ ] `feat: implement confirmation prompt`

- [ ] Delete Operation
  - [ ] `feat: add delete ammunition menu option`
  - [ ] `feat: implement delete ammunition logic`
  - [ ] `feat: add deletion confirmation prompt`
  - [ ] `feat: implement success/error messages`

- [ ] View Operation
  - [ ] `feat: enhance ammunition detail view`
  - [ ] `feat: add formatted display of ammo details`
  - [ ] `feat: implement pagination for multiple records`

### 📝 Logging System
- [ ] Basic Logging
  - [ ] `feat: create basic logging structure`
  - [ ] `feat: implement create operation logging`
  - [ ] `feat: implement update operation logging`
  - [ ] `feat: implement delete operation logging`
  - [ ] `feat: add timestamp to log entries`

### 📊 Dashboard Features
- [ ] View Options
  - [ ] `feat: add dashboard menu option`
  - [ ] `feat: implement caliber filter view`
  - [ ] `feat: implement brand filter view`
  - [ ] `feat: add quantity sorting feature`
  - [ ] `feat: implement price analysis display`

### 🔌 API Integration
- [ ] Basic Setup
  - [ ] `feat: set up basic API structure`
  - [ ] `feat: implement API authentication`
  - [ ] `docs: add API documentation`

- [ ] Endpoints
  - [ ] `feat: implement GET ammunition endpoint`
  - [ ] `feat: implement POST ammunition endpoint`
  - [ ] `feat: implement PUT ammunition endpoint`
  - [ ] `feat: add endpoint error handling`
  - [ ] `test: add API endpoint tests`

## 🛠️ Technical Requirements

### Prerequisites
- .NET 9.0
- Supabase account

### Dependencies
- Microsoft.Extensions.Configure
- Npgsql
- DotNetEnv

## 📦 Installation

1. Clone the repository
```bash
git clone https://github.com/iiSmitty/the-vault.git
cd the-vault
```

2. Set up configuration files
```bash
# Copy the example configuration file
cp appsettings.example.json appsettings.json
cp appsettings.example.json appsettings.Development.json
```

3. Configure Supabase
- Create a Supabase project
- Copy the `.env.example` to `.env`
- Update the `.env` file with your Supabase credentials:
```env
SUPABASE_CONNECTION_STRING=Host=aws-0-eu-west-2.pooler.supabase.com;Port=<your-port>;Username=<your-username>;Password=<your-password>;Database=postgres;
```

4. Build the project
```bash
dotnet build
```

5. Run the application
```bash
dotnet run
```

## 💾 Database Schema

### Ammo Table
```sql
CREATE TABLE ammo (
    id UUID PRIMARY KEY,
    caliber VARCHAR NOT NULL,
    brand VARCHAR NOT NULL,
    price DECIMAL NOT NULL,
    purchase_date DATE NOT NULL,
    quantity INTEGER NOT NULL
);
```

### Ammo_Logs Table
```sql
CREATE TABLE ammo_logs (
    id UUID PRIMARY KEY,
    ammo_id UUID REFERENCES ammo(id),
    action VARCHAR NOT NULL,
    quantity INTEGER NOT NULL,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

## 🎮 Usage

[TODO: Add Screenshots]

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📝 License
MIT

## 🙋‍♂️ Author

André Smit
- GitHub: [@iiSmitty](https://github.com/iiSmitty)

---
*Note: This project is not affiliated with or endorsed by Bethesda Softworks or the Fallout franchise.*
