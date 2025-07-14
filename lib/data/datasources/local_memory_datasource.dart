import 'package:app_map/database/database_helper.dart';
import 'package:app_map/domain/entities/memory.dart';

class LocalMemoryDatasource {
  final DatabaseHelper dbHelper;

  LocalMemoryDatasource(this.dbHelper);

  Future<void> insertMemory(Memory memory) async {
    await dbHelper.insertMemory(memory);
  }

  Future<List<Memory>> getMemories() async {
    return await dbHelper.getMemories();
  }

  Future<void> deleteMemory(int id) async {
    await dbHelper.deleteMemory(id);
  }
}
