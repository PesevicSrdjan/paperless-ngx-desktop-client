# Paperless-ngx Desktop Client

Unofficial Windows desktop client for Paperless-ngx built using REST API and WPF (.NET).

## 📌 Overview

This application provides a native Windows experience for managing documents in Paperless-ngx.
The UI is inspired by the official web interface but adapted for desktop usability.

## ✨ Features

- Dashboard overview with statistics
- Document upload support
- Document browsing and filtering
- Tags and classification system
- Integration with Paperless-ngx REST API
- Responsive desktop UI

## 🖥️ UI Preview

### Authentication
![Login](screenshots/login.png)

### Dashboard
![Dashboard](screenshots/dashboard.png)

### Documents
![Table View](screenshots/documents_table_view.png)
![Card View](screenshots/documents_card_view.png)

### Metadata
![Correspondents](screenshots/correspondents_view.png)
![Document Types](screenshots/document_types_view.png)
![Tags](screenshots/tags_view.png)

### Actions
![Upload](screenshots/upload_process_view.png)
![Preview](screenshots/documents_preview.png)

### Settings & State
![Settings](screenshots/settings_view.png)
![No Internet](screenshots/no_internet_view.png)

## 🧠 Motivation

The goal of this project was to explore Human-Computer Interaction principles by improving usability of an existing web system and translating it into a desktop environment.

## ⚙️ Tech Stack

- C#
- .NET (WPF)
- REST API
- MVVM pattern

## 📄 HCI Analysis

See `docs/HCI_Analiza_Srdan_Pesevic.pdf` for detailed analysis of UI/UX improvements compared to the web version.

## 📌 Status

This is an educational project and an unofficial client for Paperless-ngx.


## 🚀 How to run

### Option 1: Run from source
1. Clone the repository
2. Open solution in Visual Studio
3. Restore NuGet packages
4. Set API base URL in configuration
5. Run the application (F5)

### Option 2: Run executable
1. Download the latest release
2. Run `PaperlessDesktop.exe`

⚠️ This is an unofficial client and is not affiliated with Paperless-ngx.
