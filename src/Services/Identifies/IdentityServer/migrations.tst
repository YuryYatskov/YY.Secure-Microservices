add-migration Initial -Context ApplicationDbContext -Output Data/Migrations/ApplicationDb
add-migration PersistedGrantDbMigration -Context PersistedGrantDbContext -Output Data/Migrations/PersistedGrantDb
add-migration ConfigurationDbMigration -Context ConfigurationDbContext -Output Data/Migrations/ConfigurationDb

update-database -Context ApplicationDbContext
update-database -Context PersistedGrantDbContext
update-database -Context ConfigurationDbContext