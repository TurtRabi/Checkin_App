import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'dart:io';

import 'package:app_map/presentation/viewmodels/collection_viewmodel.dart';
import 'package:app_map/core/di.dart';
import 'package:app_map/domain/entities/memory.dart';

class CollectionScreen extends StatefulWidget {
  const CollectionScreen({super.key});

  @override
  State<CollectionScreen> createState() => _CollectionScreenState();
}

class _CollectionScreenState extends State<CollectionScreen> {
  late CollectionViewModel _viewModel;

  @override
  void initState() {
    super.initState();
    _viewModel = sl<CollectionViewModel>();
    _viewModel.loadMemories();
  }

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider<CollectionViewModel>.value(
      value: _viewModel,
      child: Consumer<CollectionViewModel>(
        builder: (context, viewModel, child) {
          return Scaffold(
            appBar: AppBar(
              title: const Text('Kỷ niệm của tôi'),
            ),
            body: viewModel.memories.isEmpty
                ? const Center(
                    child: Text('Chưa có kỷ niệm nào. Hãy check-in ngay!'),
                  )
                : ListView.builder(
                    itemCount: viewModel.memories.length,
                    itemBuilder: (context, index) {
                      final memory = viewModel.memories[index];
                      return Card(
                        margin: const EdgeInsets.all(8.0),
                        child: ListTile(
                          leading: Image.file(
                            File(memory.imagePath),
                            width: 80,
                            height: 80,
                            fit: BoxFit.cover,
                          ),
                          title: Text(memory.locationName),
                          subtitle: Text('${memory.description} - Mức độ an toàn: ${memory.safetyLevel} - Độ hiếm: ${memory.rarity} - ${memory.timestamp.toLocal().toString().split('.')[0]}'),
                          trailing: IconButton(
                            icon: const Icon(Icons.delete, color: Colors.red),
                            onPressed: () => viewModel.deleteMemory(memory.id!),
                          ),
                          onTap: () {
                            // TODO: Hiển thị chi tiết kỷ niệm trên bản đồ hoặc màn hình riêng
                          },
                        ),
                      );
                    },
                  ),
          );
        },
      ),
    );
  }
}