import 'package:app_map/domain/repositories/memory_repository.dart';
import 'package:app_map/domain/entities/memory.dart';
import 'package:app_map/data/datasources/local_memory_datasource.dart';

class MemoryRepositoryImpl implements MemoryRepository {
  final LocalMemoryDatasource localDatasource;

  MemoryRepositoryImpl(this.localDatasource);

  @override
  Future<void> insertMemory(Memory memory) {
    return localDatasource.insertMemory(memory);
  }

  @override
  Future<List<Memory>> getMemories() {
    return localDatasource.getMemories();
  }

  @override
  Future<void> deleteMemory(int id) {
    return localDatasource.deleteMemory(id);
  }
}
