# SimpleQuery Core
ORM Very Easy to use. All CRUD operations. Look:

Nuget

PM: Install-Package SimpleQuery-ORM 

Paket CLI: paket add SimpleQuery-ORM 

Execute insert, update, delete, select returing strongly typed List
------------------------------------------------------------
```csharp
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }
     
    var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["sqlserver"].ConnectionString);    
    var cliente = new Cliente() { Nome = "Miranda" };        
    var id = conn.InsertReturningId<Cliente>(cliente);              
```
Example usage select (GetAll):
```csharp
    var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString);
    var clientes = conn.GetAll<Cliente>();
```

Example usage update:
```csharp
    var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString);
    var cliente = new Cliente() { Nome = "Miranda" };
    conn.Update<Cliente>(cliente);
```
Example usage delete:
```csharp
    var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString);
    var cliente = new Cliente() {Id=1, Nome = "Miranda" };
    conn.Delete(cliente);
```

     
     
