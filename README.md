# ePermitsApp
Unified e-Permits

# Architecture

This project uses the Repository + Service pattern.

* Controllers: handle HTTP only
* Services: contain business logic
* Data: contains DbContext and repositories
* Models: domain entities

Rules:
* Controllers never access DbContext
* Services are injected via interfaces
* Models have no business logic

# Database Schema 

https://docs.google.com/document/d/1_IpXLIycEwtIaADiaR04MSGNz1Lh1BPT_KNcJfcGuI4/edit?usp=sharing

# Project Structure (Clean & Scalable)

LocationService.Api<br>
│<br>
├── Controllers<br>
│   └── ProvincesController.cs<br>
│<br>
├── Services<br>
│   ├── Interfaces<br>
│   │   └── IProvinceService.cs<br>
│   └── ProvinceService.cs<br>
│<br>
├── Repositories<br>
│   ├── Interfaces<br>
│   │   └── IProvinceRepository.cs<br>
│   └── ProvinceRepository.cs<br>
│<br>
├── Data<br>
│   ├── ApplicationDbContext.cs<br>
│<br>
├── Entities<br>
│   └── Province.cs<br>
│<br>
├── DTOs<br>
│   └── ProvinceDto.cs<br>
│<br>
├── Mappings<br>
│   └── MappingProfile.cs<br>
│<br>
└── Program.cs<br>


# Responsibility Split
Layer <---> Responsibility

* Controller --> HTTP concerns
* Service --> Business rules
* Repository --> Data access