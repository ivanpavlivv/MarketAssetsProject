# MarketAssetsAPI
A REST API service that provides real-time and historical price information for market assets

## Overview
The service exposes three REST endpoints:
- GET /api/assets — returns a list of all supported market assets
- GET /api/assets/prices — returns current price information for one or more assets
- GET /api/assets/history - returns historical prices. Accepts 3 params: symbol - symbol for specific asset, from - start of date range from which we will get historical prices, to - end of date range from which we will get historical prices 

## Requirements
Docker Desktop is required to run this project

## Running the project
1. Clone repository

2. Start the application via Docker
docker compose up --build