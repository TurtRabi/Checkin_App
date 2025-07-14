import 'package:flutter/material.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:app_map/domain/entities/memory.dart';
import 'package:app_map/domain/usecases/get_current_location.dart';
import 'package:app_map/domain/usecases/get_memories.dart';
import 'package:app_map/domain/usecases/get_directions.dart'; // New
import 'dart:math';

class MapViewModel extends ChangeNotifier {
  final GetCurrentLocation _getCurrentLocation;
  final GetMemories _getMemories;
  final GetDirections _getDirections; // New

  MapViewModel(this._getCurrentLocation, this._getMemories, this._getDirections); // Updated constructor

  LatLng? _currentLocation;
  LatLng? get currentLocation => _currentLocation;

  Set<Marker> _markers = {};
  Set<Marker> get markers => _markers;

  List<Memory> _allMemories = [];
  List<Memory> get allMemories => _allMemories;
  List<Memory> _filteredMemories = [];

  String _searchQuery = '';
  String get searchQuery => _searchQuery;
  String _selectedTypeFilter = '';
  String get selectedTypeFilter => _selectedTypeFilter;
  String _selectedSafetyFilter = '';
  String get selectedSafetyFilter => _selectedSafetyFilter;
  bool _showVisited = false;

  Set<Polyline> _polylines = {}; // New
  Set<Polyline> get polylines => _polylines; // New

  Future<void> loadInitialData() async {
    _currentLocation = await _getCurrentLocation();
    await _loadMemories();
    notifyListeners();
  }

  Future<void> _loadMemories() async {
    _allMemories = await _getMemories();
    _applyFilters();
  }

  void _applyFilters() {
    _filteredMemories = _allMemories.where((memory) {
      final matchesSearch = memory.locationName.toLowerCase().contains(_searchQuery.toLowerCase()) ||
          memory.description.toLowerCase().contains(_searchQuery.toLowerCase());

      final matchesType = _selectedTypeFilter == 'Tất cả' ||
          (memory.description.toLowerCase().contains(_selectedTypeFilter.toLowerCase())); // Tạm thời lọc theo mô tả

      final matchesSafety = _selectedSafetyFilter == 'Tất cả' ||
          memory.safetyLevel == _selectedSafetyFilter;

      // TODO: Implement visited logic later
      final matchesVisited = true; // For now, always true

      return matchesSearch && matchesType && matchesSafety && matchesVisited;
    }).toList();
    _updateMarkers();
  }

  void setSearchQuery(String query) {
    _searchQuery = query;
    _applyFilters();
  }

  void setSelectedTypeFilter(String filter) {
    _selectedTypeFilter = filter;
    _applyFilters();
  }

  void setSelectedSafetyFilter(String filter) {
    _selectedSafetyFilter = filter;
    _applyFilters();
  }

  void setShowVisited(bool value) {
    _showVisited = value;
    _applyFilters();
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

    for (var memory in _filteredMemories) {
      _markers.add(
        Marker(
          markerId: MarkerId(memory.id.toString()),
          position: LatLng(memory.latitude, memory.longitude),
          infoWindow: InfoWindow(
            title: memory.locationName,
            snippet: '${memory.description} - Mức độ an toàn: ${memory.safetyLevel}',
            // onTap: () => _showMemoryDetails(memory), // Sẽ xử lý trong View
          ),
          icon: BitmapDescriptor.defaultMarkerWithHue(BitmapDescriptor.hueOrange),
        ),
      );
    }
    notifyListeners();
  }

  Future<void> getRoute(LatLng destination) async {
    print('MapViewModel: getRoute được gọi.');
    if (_currentLocation == null) {
      print('MapViewModel: Vị trí hiện tại null, không thể lấy lộ trình.');
      return;
    }
    _polylines.clear();
    print('MapViewModel: Đang lấy lộ trình từ ${_currentLocation!.latitude},${_currentLocation!.longitude} đến ${destination.latitude},${destination.longitude}');
    final points = await _getDirections(_currentLocation!, destination);
    print('MapViewModel: Đã nhận được ${points.length} điểm lộ trình.');
    _polylines.add(Polyline(
      polylineId: const PolylineId('route'),
      points: points,
      color: Colors.blue,
      width: 5,
    ));
    notifyListeners();
    print('MapViewModel: Đã cập nhật polylines và thông báo người nghe.');
  }

  // Helper to calculate distance (simplified for now)
  double calculateDistance(LatLng start, LatLng end) {
    // Using Haversine formula for accurate distance calculation
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

    return R * c / 1000; // Distance in kilometers
  }

  double _toRadians(double degree) {
    return degree * pi / 180;
  }
}