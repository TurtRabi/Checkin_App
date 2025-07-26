import 'dart:math';

import 'package:flutter/material.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:image_picker/image_picker.dart';
import 'package:path_provider/path_provider.dart';
import 'dart:io';

import 'package:app_map/domain/entities/memory.dart';
import 'package:app_map/domain/usecases/get_current_location.dart';
import 'package:app_map/domain/usecases/generate_random_location.dart';
import 'package:app_map/domain/usecases/insert_memory.dart';
import 'package:app_map/domain/usecases/get_memories.dart';

import 'package:app_map/domain/usecases/delete_memory.dart';

class CheckInViewModel extends ChangeNotifier {
  final GetCurrentLocation _getCurrentLocation;
  final GenerateRandomLocation _generateRandomLocation;
  final InsertMemory _insertMemory;
  final GetMemories _getMemories;
  final DeleteMemory _deleteMemory;

  CheckInViewModel(
    this._getCurrentLocation,
    this._generateRandomLocation,
    this._insertMemory,
    this._getMemories,
    this._deleteMemory,
  );

  LatLng? _currentLocation;
  LatLng? get currentLocation => _currentLocation;

  Set<Marker> _markers = {};
  Set<Marker> get markers => _markers;

  List<Memory> _memories = [];

  final List<String> _rarityLevels = ['Thường', 'Hiếm', 'Huyền thoại'];

  String _getRandomRarity() {
    final random = Random();
    final int randomNumber = random.nextInt(100); // 0-99

    if (randomNumber < 70) { // 70% chance
      return 'Thường';
    } else if (randomNumber < 95) { // 25% chance (70-94)
      return 'Hiếm';
    } else { // 5% chance (95-99)
      return 'Huyền thoại';
    }
  }

  // Helper to calculate distance (Haversine formula)
  double calculateDistance(LatLng start, LatLng end) {
    const double R = 6371e3; // Earth's radius in meters
    final double lat1 = _toRadians(start.latitude);
    final double lon1 = _toRadians(start.longitude);
    final double lat2 = _toRadians(end.latitude);
    final double lon2 = _toRadians(end.longitude);

    final double dLat = lat2 - lat1;
    final double dLon = lon2 - lon1;

    final double a = sin(dLat / 2) * sin(dLat / 2) +
        cos(lat1) * cos(lat2) * sin(dLon / 2) * sin(dLon / 2);
    final double c = 2 * atan2(sqrt(a), sqrt(1 - a));

    return R * c; // Distance in meters
  }

  double _toRadians(double degree) {
    return degree * pi / 180;
  }

  Future<void> loadInitialData() async {
    _currentLocation = await _getCurrentLocation();
    await _loadMemories();
    notifyListeners();
  }

  Future<void> _loadMemories() async {
    _memories = await _getMemories();
    _updateMarkers();
  }

  void _updateMarkers() {
    _markers.clear();
    if (_currentLocation != null) {
      _markers.add(
        Marker(
          markerId: const MarkerId('currentLocation'),
          position: _currentLocation!,
          infoWindow: const InfoWindow(title: 'Vị trí hiện tại của tôi'),
          icon: BitmapDescriptor.defaultMarkerWithHue(BitmapDescriptor.hueAzure),
        ),
      );
    }

    for (var memory in _memories) {
      _markers.add(
        Marker(
          markerId: MarkerId(memory.id.toString()),
          position: LatLng(memory.latitude, memory.longitude),
          infoWindow: InfoWindow(
            title: 'Kỷ niệm: ${memory.description}',
            snippet: 'Mức độ an toàn: ${memory.safetyLevel} - ${memory.timestamp.toLocal().toString().split('.')[0]}',
            // onTap: () => _showMemoryDetails(memory), // Sẽ xử lý trong View
          ),
          icon: BitmapDescriptor.defaultMarkerWithHue(BitmapDescriptor.hueOrange),
        ),
      );
    }
    notifyListeners();
  }

  Future<Map<String, dynamic>?> checkInRandomLocation() async {
    if (_currentLocation == null) {
      return null; // Xử lý lỗi ở View
    }

    final Map<String, dynamic> randomLocationData = await _generateRandomLocation(
      _currentLocation!,
      20.0, // 20km radius
    );
    return randomLocationData;
  }

  Future<Memory?> takePictureAndSaveMemory(LatLng location, String safetyLevel, String locationName, BuildContext context) async {
    final ImagePicker _picker = ImagePicker();
    final XFile? photo = await _picker.pickImage(source: ImageSource.camera);

    if (photo != null) {
      final appDir = await getApplicationDocumentsDirectory();
      final String fileName = '${DateTime.now().millisecondsSinceEpoch}.png';
      final String localPath = '${appDir.path}/$fileName';
      final File newImage = await File(photo.path).copy(localPath);

      String? description = await showDialog<String>(
        context: context,
        builder: (BuildContext context) {
          String? inputDescription;
          return AlertDialog(
            title: const Text('Thêm mô tả kỷ niệm'),
            content: TextField(
              onChanged: (value) {
                inputDescription = value;
              },
              decoration: const InputDecoration(hintText: 'Nhập mô tả kỷ niệm của bạn'),
            ),
            actions: <Widget>[
              TextButton(
                onPressed: () => Navigator.of(context).pop(),
                child: const Text('Hủy'),
              ),
              TextButton(
                onPressed: () => Navigator.of(context).pop(inputDescription),
                child: const Text('Lưu'),
              ),
            ],
          );
        },
      );

      if (description != null && description.isNotEmpty) {
        final newMemory = Memory(
          latitude: location.latitude,
          longitude: location.longitude,
          imagePath: newImage.path,
          description: description,
          timestamp: DateTime.now(),
          safetyLevel: safetyLevel,
          locationName: locationName,
          rarity: _getRandomRarity(),
        );
        await _insertMemory(newMemory);
        await _loadMemories();
        return newMemory;
      } else {
        return null;
      }
    } else {
      return null;
    }
  }

  void showMemoryDetails(Memory memory, BuildContext context) {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: Text(memory.locationName),
          content: SingleChildScrollView(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Image.file(File(memory.imagePath)),
                Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: Text('Mô tả: ${memory.description}'),
                ),
                Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: Text('Thời gian: ${memory.timestamp.toLocal().toString().split('.')[0]}'),
                ),
                Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: Text('Tọa độ: ${memory.latitude.toStringAsFixed(4)}, ${memory.longitude.toStringAsFixed(4)}'),
                ),
                Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: Text('Mức độ an toàn: ${memory.safetyLevel}', style: TextStyle(fontWeight: FontWeight.bold)),
                ),
              ],
            ),
          ),
          actions: <Widget>[
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text('Đóng'),
            ),
            TextButton(
              onPressed: () async {
                await _deleteMemory(memory.id!);
                Navigator.of(context).pop();
                await _loadMemories();
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(content: Text('Kỷ niệm đã được xóa.')),
                );
              },
              child: const Text('Xóa', style: TextStyle(color: Colors.red)),
            ),
          ],
        );
      },
    );
  }
}
