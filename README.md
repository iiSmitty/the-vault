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

### Current Features
- Track multiple ammunition types with detailed information:
  - Caliber
  - Brand
  - Purchase price
  - Purchase date
  - Quantity
- Comprehensive logging system that records:
  - All ammunition transactions
  - Action types
  - Quantity changes
  - Timestamps
- Fallout-inspired terminal interface with:
  - Custom ASCII art
  - Pip-Boy-style green text
  - Retro animations
- Persistent data storage using Supabase

### 🚀 Planned Features
- Sorting and filtering capabilities
- Dashboard view for overall ammunition status
- Full CRUD operations
- RESTful API endpoints for remote updates

## 🛠️ Technical Requirements

### Prerequisites
- .NET 9.0
- Supabase account

### Dependencies
- Microsoft.Extensions.Configure
- Npgsql

## 📦 Installation

1. Clone the repository
```bash
git clone https://github.com/iiSmitty/the-vault.git
```

2. Configure Supabase
- Create a Supabase project
- Copy the `.env.example` to `.env`
- Update the `.env` file with your Supabase credentials:
```env
SUPABASE_CONNECTION_STRING=Host=aws-0-eu-west-2.pooler.supabase.com;Port=<your-port>;Username=<your-username>;Password=<your-password>;Database=postgres;
```

3. Build the project
```bash
dotnet build
```

4. Run the application
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

André ASmit
- GitHub: [@iiSmitty](https://github.com/iiSmitty)

---
*Note: This project is not affiliated with or endorsed by Bethesda Softworks or the Fallout franchise.*
