dotnet.exe ef dbcontext scaffold "Server=(local);Database=SummarisedActuals;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -c SummarisationContext --schema dbo --force --startup-project . --project ..\ESFA.DC.Summarisation.Model --verbose