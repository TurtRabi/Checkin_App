import 'package:app_map/domain/repositories/memory_repository.dart';

class DeleteMemory {
  final MemoryRepository repository;

  DeleteMemory(this.repository);

  Future<void> call(int id) {
    return repository.deleteMemory(id);
  }
}
