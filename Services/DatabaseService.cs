using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RitiksJournalApp.Models;
using SQLite;
using SQLitePCL;

namespace RitiksJournalApp.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;

        // 1. Initialize the Database
        async Task Init()
        {
            if (_database is not null)
                return;

            // This creates a file named "Journal.db" in the app's local storage
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "Journal.db");

            _database = new SQLiteAsyncConnection(dbPath);

            // Create the table for Journal Entries if it doesn't exist
            await _database.CreateTableAsync<JournalEntry>();
        }

        // 2. GET all entries (Task 6: Paginated View)
        public async Task<List<JournalEntry>> GetEntriesAsync()
        {
            await Init();
            // Get all entries, sorted by newest date first
            return await _database!.Table<JournalEntry>().OrderByDescending(e => e.Date).ToListAsync();
        }

        // 3. GET one entry by ID
        public async Task<JournalEntry> GetEntryByIdAsync(int id)
        {
            await Init();
            return await _database!.Table<JournalEntry>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        // 4. GET entry by Date (Task 1: One entry per day rule)
        public async Task<JournalEntry> GetEntryByDateAsync(DateTime date)
        {
            await Init();
            // We compare just the Date part (ignoring time)
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddDays(1).AddTicks(-1);

            return await _database!.Table<JournalEntry>()
                            .Where(e => e.Date >= startOfDay && e.Date <= endOfDay)
                            .FirstOrDefaultAsync();
        }

        // 5. SAVE or UPDATE (Task 1: Create/Update)
        public async Task SaveEntryAsync(JournalEntry entry)
        {
            await Init();
            if (entry.Id != 0)
            {
                // Update existing
                entry.UpdatedAt = DateTime.Now;
                await _database!.UpdateAsync(entry);
            }
            else
            {
                // Create new
                entry.CreatedAt = DateTime.Now;
                entry.UpdatedAt = DateTime.Now;
                await _database!.InsertAsync(entry);
            }
        }

        // 6. DELETE (Task 1: Delete)
        // NEW: Deletes by ID number instead of requiring the whole object
        public async Task DeleteEntryAsync(int id)
        {
            await Init();
            // This tells the database: "Find the JournalEntry with this ID and delete it"
            await _database!.DeleteAsync<JournalEntry>(id);
        }
    }
}
