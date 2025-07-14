import 'package:flutter/material.dart';
import 'package:app_map/domain/entities/memory.dart';
import 'package:app_map/domain/usecases/get_memories.dart';
import 'package:app_map/domain/usecases/delete_memory.dart';

class CollectionViewModel extends ChangeNotifier {
  final GetMemories _getMemories;
  final DeleteMemory _deleteMemory;

  CollectionViewModel(this._getMemories, this._deleteMemory);

  List<Memory> _memories = [];
  List<Memory> get memories => _memories;

  int get totalCards => _memories.length;

  int get uniqueLocations => _memories.map((m) => m.locationName).toSet().length;

  int get rareCards => _memories.where((m) => m.rarity == 'Hiếm' || m.rarity == 'Huyền thoại').length;

  Future<void> loadMemories() async {
    _memories = await _getMemories();
    notifyListeners();
  }

  Future<void> deleteMemory(int id) async {
    await _deleteMemory(id);
    await loadMemories(); // Reload memories after deletion
  }
}
