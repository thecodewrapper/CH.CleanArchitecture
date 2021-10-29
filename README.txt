Run the following to update/install databases: 

In Package Manager with CH.CleanArchitecture.Infrastructure.Data selected as the Default Project, and have CH.CleanArchitecture.Presentation.Web set as solution startup project

update-database -context ApplicationDbContext
update-database -context EventStoreDbContext
update-database -context IdentityDbContext