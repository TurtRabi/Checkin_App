import 'package:flutter/material.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:provider/provider.dart';

import 'package:app_map/presentation/viewmodels/map_viewmodel.dart';
import 'package:app_map/core/di.dart';
import 'package:app_map/domain/entities/memory.dart';
import 'package:url_launcher/url_launcher.dart';

class MapScreen extends StatefulWidget {
  const MapScreen({super.key});

  @override
  State<MapScreen> createState() => _MapScreenState();
}

class _MapScreenState extends State<MapScreen> {
  late MapViewModel _viewModel;
  GoogleMapController? _mapController;
  final TextEditingController _searchController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _viewModel = sl<MapViewModel>();
    _viewModel.loadInitialData();
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  void _onMapCreated(GoogleMapController controller) {
    _mapController = controller;
  }

  void _showMemoryDetails(Memory memory) {
    showModalBottomSheet(
      context: context,
      builder: (BuildContext context) {
        return Container(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                memory.locationName,
                style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 8),
              Text(
                'Cách bạn ${(_viewModel.currentLocation != null ? _viewModel.calculateDistance(_viewModel.currentLocation!, LatLng(memory.latitude, memory.longitude)) : 0).toStringAsFixed(2)} km - Mức độ an toàn: ${memory.safetyLevel}',
                style: const TextStyle(fontSize: 14, color: Colors.grey),
              ),
              const SizedBox(height: 8),
              Text(
                memory.description,
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
              ),
              const SizedBox(height: 16),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceAround,
                children: [
                  ElevatedButton(
                    onPressed: () async {
                      print('Nút "Chỉ đường nội bộ" được nhấn.');
                      Navigator.pop(context);
                      await _viewModel.getRoute(LatLng(memory.latitude, memory.longitude));
                      _mapController?.animateCamera(CameraUpdate.newLatLngZoom(LatLng(memory.latitude, memory.longitude), 15.0));
                    },
                    child: const Text('Chỉ đường nội bộ'),
                  ),
                  ElevatedButton(
                    onPressed: () async {
                      final url = 'https://www.google.com/maps/dir/?api=1&destination=${memory.latitude},${memory.longitude}';
                      if (await canLaunchUrl(Uri.parse(url))) {
                        await launchUrl(Uri.parse(url));
                      } else {
                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(content: Text('Không thể mở Google Maps.')),
                        );
                      }
                    },
                    child: const Text('Chỉ đường Google Maps'),
                  ),
                ],
              ),
            ],
          ),
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider<MapViewModel>.value(
      value: _viewModel,
      child: Consumer<MapViewModel>(
        builder: (context, viewModel, child) {
          return Scaffold(
            appBar: AppBar(
              title: const Text('Bản đồ'),
              bottom: PreferredSize(
                preferredSize: const Size.fromHeight(100.0),
                child: Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: Column(
                    children: [
                      TextField(
                        controller: _searchController,
                        decoration: InputDecoration(
                          hintText: 'Tìm địa danh...',
                          prefixIcon: const Icon(Icons.search),
                          border: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(8.0),
                            borderSide: BorderSide.none,
                          ),
                          filled: true,
                          fillColor: Colors.white,
                        ),
                        onChanged: (query) {
                          viewModel.setSearchQuery(query);
                        },
                      ),
                      const SizedBox(height: 8),
                      SingleChildScrollView(
                        scrollDirection: Axis.horizontal,
                        child: Row(
                          children: [
                            FilterChip(
                              label: const Text('Tất cả'),
                              selected: viewModel.selectedTypeFilter == 'Tất cả',
                              onSelected: (selected) {
                                viewModel.setSelectedTypeFilter(selected ? 'Tất cả' : '');
                              },
                            ),
                            const SizedBox(width: 8),
                            FilterChip(
                              label: const Text('Lịch sử'),
                              selected: viewModel.selectedTypeFilter == 'Lịch sử',
                              onSelected: (selected) {
                                viewModel.setSelectedTypeFilter(selected ? 'Lịch sử' : '');
                              },
                            ),
                            const SizedBox(width: 8),
                            FilterChip(
                              label: const Text('Văn hóa'),
                              selected: viewModel.selectedTypeFilter == 'Văn hóa',
                              onSelected: (selected) {
                                viewModel.setSelectedTypeFilter(selected ? 'Văn hóa' : '');
                              },
                            ),
                            const SizedBox(width: 8),
                            FilterChip(
                              label: const Text('Thiên nhiên'),
                              selected: viewModel.selectedTypeFilter == 'Thiên nhiên',
                              onSelected: (selected) {
                                viewModel.setSelectedTypeFilter(selected ? 'Thiên nhiên' : '');
                              },
                            ),
                            const SizedBox(width: 8),
                            FilterChip(
                              label: const Text('An toàn'),
                              selected: viewModel.selectedSafetyFilter == 'An toàn',
                              onSelected: (selected) {
                                viewModel.setSelectedSafetyFilter(selected ? 'An toàn' : '');
                              },
                            ),
                            const SizedBox(width: 8),
                            FilterChip(
                              label: const Text('Cảnh báo'),
                              selected: viewModel.selectedSafetyFilter == 'Cảnh báo',
                              onSelected: (selected) {
                                viewModel.setSelectedSafetyFilter(selected ? 'Cảnh báo' : '');
                              },
                            ),
                            const SizedBox(width: 8),
                            FilterChip(
                              label: const Text('Nguy hiểm'),
                              selected: viewModel.selectedSafetyFilter == 'Nguy hiểm',
                              onSelected: (selected) {
                                viewModel.setSelectedSafetyFilter(selected ? 'Nguy hiểm' : '');
                              },
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
            body: GoogleMap(
              onMapCreated: _onMapCreated,
              initialCameraPosition: CameraPosition(
                target: viewModel.currentLocation ?? const LatLng(10.762622, 106.660172),
                zoom: 15.0,
              ),
              myLocationEnabled: true,
              myLocationButtonEnabled: true,
              markers: viewModel.markers,
              polylines: viewModel.polylines,
            ),
          );
        },
      ),
    );
  }
}