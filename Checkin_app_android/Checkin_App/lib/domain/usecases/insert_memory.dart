import 'package:app_map/domain/entities/memory.dart';
import 'package:app_map/domain/repositories/memory_repository.dart';

class InsertMemory {
  final MemoryRepository repository;

  InsertMemory(this.repository);

  Future<void> call(Memory memory) {
    return repository.insertMemory(memory);
  }
}
