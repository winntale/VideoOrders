how to run ts

в докере
cd OneDrive/прога/VideoOrders
docker compose up -d

в конфигах стырить коннекшн стрингу и в дбивере (или любая другая субд где есть постгра) натянуть ее

dotnet ef migrations add InitialCreate --project OrderService/Implementations/Dal --startup-project OrderService/Gateway --context OrderDbContext
dotnet ef database update --project OrderService/Implementations/Dal --startup-project OrderService/Gateway --context OrderDbContext

dotnet ef migrations add InitialCreate --project UserService/Implementations/Dal --startup-project UserService/Gateway --context UserDbContext
dotnet ef database update --project UserService/Implementations/Dal --startup-project UserService/Gateway --context UserDbContext

dotnet ef migrations add InitialCreate --project VideoArchiveService/Implementations/Dal --startup-project VideoArchiveService/Gateway --context VideoArchiveDbContext
dotnet ef database update --project VideoArchiveService/Implementations/Dal --startup-project VideoArchiveService/Gateway --context VideoArchiveDbContext

всё билди пользуйся
