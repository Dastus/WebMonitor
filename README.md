# WebMonitor
App for realtime web app monitoring

On FE:
- Angular web UI app displays check statuses list and allows to run it manually
- ngrx used to store current state

On BE:
- different kinds of checks for Web App monitoring implemented: Web UI selenium checks, HTML structure checks, API checks 
- external unit tests (MsTest, nUnit) and Selenium UI tests execution mechanism implemented
- each check runs on its own schedule and can be stopped or re-scheduled via API
- CQRS approach has been used to divide Commands and Queries
- MediatR pipeline behavior used to create different pipelines for Commands and Queries processing. Commands decorated to store check results, save logs, send Telegram and SignaR notifications

Clean architecture approach described by Jason Taylor has been used (https://github.com/JasonGT/NorthwindTraders).
