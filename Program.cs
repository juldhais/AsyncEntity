using System.Diagnostics;
using AsyncEntity;
using Microsoft.EntityFrameworkCore;

var stopwatch = new Stopwatch();

var connectionString = "Data Source=.\\SQL2019;Initial Catalog=AsyncEntity;Integrated Security=True";
var optionsBuilder = new DbContextOptionsBuilder().UseSqlServer(connectionString);

// initialize data
// stopwatch.Start();
// var dbInitializer = new DataContext(optionsBuilder.Options);
// var dataInitializer = new DataInitializer(dbInitializer);
// await dataInitializer.Run();

// stopwatch.Stop();
// Console.WriteLine($"Data initialization: {stopwatch.Elapsed}");

// synchronous
stopwatch.Restart();
var dbSync = new DataContext(optionsBuilder.Options);

var syncSuppliers = dbSync.Supplier
    .Where(x => x.Name.Contains("7"))
    .OrderByDescending(x => x.Name)
    .ToList();

var syncCustomer = dbSync.Customer
    .Where(x => x.Name.Contains("7"))
    .OrderByDescending(x => x.Name)
    .ToList();

var syncTotal = syncSuppliers.Count + syncCustomer.Count;
stopwatch.Stop();
Console.WriteLine($"Synchronous (Total: {syncTotal}): {stopwatch.Elapsed}");

// sequential
stopwatch.Restart();
var dbSequential = new DataContext(optionsBuilder.Options);

var sequentialSuppliers = await dbSequential.Supplier
    .Where(x => x.Name.Contains("7"))
    .OrderByDescending(x => x.Name)
    .ToListAsync();

var sequentialCustomers = await dbSequential.Customer
    .Where(x => x.Name.Contains("7"))
    .OrderByDescending(x => x.Name)
    .ToListAsync();

var sequentialTotal = sequentialSuppliers.Count + sequentialCustomers.Count;
stopwatch.Stop();
Console.WriteLine($"Sequential (Total: {sequentialTotal}): {stopwatch.Elapsed}");

// parallel
stopwatch.Restart();

// buat dua buah instance DataContext
var dbParallel1 = new DataContext(optionsBuilder.Options);
var dbParallel2 = new DataContext(optionsBuilder.Options);

// task untuk query pertama (tidak perlu di await)
var parallelSupplierTask = dbParallel1.Supplier    
    .Where(x => x.Name.Contains("7"))
    .OrderByDescending(x => x.Name)
    .ToListAsync();

// task untuk query kedua (tidak perlu di await)
var parallelCustomerTask = dbParallel2.Customer
    .Where(x => x.Name.Contains("7"))
    .OrderByDescending(x => x.Name)
    .ToListAsync();

// Task.WhenAll akan menjalankan kedua task secara parallel hingga selesai (await disini)
await Task.WhenAll(parallelSupplierTask, parallelCustomerTask);

// ambil hasil dari kedua query
var parallelSuppliers = parallelSupplierTask.Result;
var parallelCustomers = parallelCustomerTask.Result;

var parallelTotal = parallelSuppliers.Count + parallelCustomers.Count;
stopwatch.Stop();
Console.WriteLine($"Parallel (Total: {parallelTotal}): {stopwatch.Elapsed}");

Console.ReadKey();


