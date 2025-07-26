import 'package:app_map/domain/entities/memory.dart';

abstract class MemoryRepository {
  Future<void> insertMemory(Memory memory);
  Future<List<Memory>> getMemories();
  Future<void> deleteMemory(int id);
}
