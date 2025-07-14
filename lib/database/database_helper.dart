import 'package:sqflite/sqflite.dart';
import 'package:path/path.dart';
import '../domain/entities/memory.dart';


class DatabaseHelper {
  static final DatabaseHelper _instance = DatabaseHelper._internal();
  static Database? _database;

  factory DatabaseHelper() {
    return _instance;
  }

  DatabaseHelper._internal();

  Future<Database> get database async {
    if (_database != null) return _database!;
    _database = await _initDb();
    return _database!;
  }

  Future<Database> _initDb() async {
    String path = join(await getDatabasesPath(), 'memories.db');
    return await openDatabase(
      path,
      version: 4, // Tăng phiên bản cơ sở dữ liệu
      onCreate: (db, version) {
        return db.execute(
          "CREATE TABLE memories(id INTEGER PRIMARY KEY AUTOINCREMENT, latitude REAL, longitude REAL, imagePath TEXT, description TEXT, timestamp TEXT, safetyLevel TEXT, locationName TEXT, rarity TEXT)",
        );
      },
      onUpgrade: (db, oldVersion, newVersion) async {
        if (oldVersion < 2) {
          await db.execute("ALTER TABLE memories ADD COLUMN safetyLevel TEXT DEFAULT 'An toàn'");
        }
        if (oldVersion < 3) {
          await db.execute("ALTER TABLE memories ADD COLUMN locationName TEXT DEFAULT 'Địa điểm không tên'");
        }
        if (oldVersion < 4) {
          await db.execute("ALTER TABLE memories ADD COLUMN rarity TEXT DEFAULT 'Thường'");
        }
      },
    );
  }

  Future<void> insertMemory(Memory memory) async {
    final db = await database;
    await db.insert(
      'memories',
      memory.toMap(),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
  }

  Future<List<Memory>> getMemories() async {
    final db = await database;
    final List<Map<String, dynamic>> maps = await db.query('memories', orderBy: 'timestamp DESC');
    return List.generate(maps.length, (i) {
      return Memory.fromMap(maps[i]);
    });
  }

  Future<void> deleteMemory(int id) async {
    final db = await database;
    await db.delete(
      'memories',
      where: 'id = ?',
      whereArgs: [id],
    );
  }
}
