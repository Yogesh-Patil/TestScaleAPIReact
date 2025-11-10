using System.Data;
using Microsoft.Data.SqlClient;
using System.Text;

var conn = Environment.GetEnvironmentVariable("ConnectionString") ?? throw new Exception("ConnectionString env required");
var rows = int.Parse(Environment.GetEnvironmentVariable("ROWS") ?? "500");
var minPayload = int.Parse(Environment.GetEnvironmentVariable("MIN_PAYLOAD") ?? "256");
var maxPayload = int.Parse(Environment.GetEnvironmentVariable("MAX_PAYLOAD") ?? "2048");
var batchSize = int.Parse(Environment.GetEnvironmentVariable("BATCH_SIZE") ?? "5000");

Console.WriteLine($"Seeder starting: rows={rows}, payload={minPayload}-{maxPayload}, batch={batchSize}");

using (var connection = new SqlConnection(conn))
{
    await connection.OpenAsync();
    var create = @"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
BEGIN
  CREATE TABLE [dbo].[Products](
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(250) NOT NULL,
    [Payload] NVARCHAR(MAX) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
  )
END
";
    using var cmd = new SqlCommand(create, connection);
    await cmd.ExecuteNonQueryAsync();
    connection.Close();
}

var rand = new Random();
int inserted = 0;
var sw = System.Diagnostics.Stopwatch.StartNew();
while (inserted < rows)
{
    var thisBatch = Math.Min(batchSize, rows - inserted);
    var dt = new DataTable();
    dt.Columns.Add("Name", typeof(string));
    dt.Columns.Add("Payload", typeof(string));
    dt.Columns.Add("CreatedAt", typeof(DateTime));

    for (int i=0;i<thisBatch;i++)
    {
        var idx = inserted + i + 1;
        var size = rand.Next(minPayload, maxPayload+1);
        dt.Rows.Add($"Product {idx}", RandomString(size, rand), DateTime.UtcNow);
    }
    await WriteBatch(conn, dt);
    inserted += thisBatch;
    Console.WriteLine($"Inserted {inserted}/{rows}");
}
sw.Stop();
Console.WriteLine($"Done. Time={sw.Elapsed}");

static async Task WriteBatch(string conn, DataTable dt)
{
    using var connection = new SqlConnection(conn);
    await connection.OpenAsync();
    using var bulk = new SqlBulkCopy(connection)
    {
        DestinationTableName = "dbo.Products",
        BatchSize = dt.Rows.Count,
        BulkCopyTimeout = 0
    };
    bulk.ColumnMappings.Add("Name","Name");
    bulk.ColumnMappings.Add("Payload","Payload");
    bulk.ColumnMappings.Add("CreatedAt","CreatedAt");
    await bulk.WriteToServerAsync(dt);
    await connection.CloseAsync();
}

static string RandomString(int size, Random r)
{
    const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
    var sb = new StringBuilder(size);
    for (int i=0;i<size;i++) sb.Append(chars[r.Next(chars.Length)]);
    return sb.ToString();
}
