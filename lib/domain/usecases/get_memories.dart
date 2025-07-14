import 'package:app_map/domain/entities/memory.dart';
import 'package:app_map/domain/repositories/memory_repository.dart';

class GetMemories {
  final MemoryRepository repository;

  GetMemories(this.repository);

  Future<List<Memory>> call() {
    return repository.getMemories();
  }
}
