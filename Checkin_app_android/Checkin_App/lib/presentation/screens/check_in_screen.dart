import 'package:flutter/material.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:provider/provider.dart';
import 'package:url_launcher/url_launcher.dart';
import 'dart:io';

import 'package:app_map/presentation/viewmodels/check_in_viewmodel.dart';
import 'package:app_map/core/di.dart';
import 'package:app_map/domain/entities/memory.dart';

class CheckInScreen extends StatefulWidget {
  const CheckInScreen({super.key});

  @override
  State<CheckInScreen> createState() => _CheckInScreenState();
}

class _CheckInScreenState extends State<CheckInScreen> with SingleTickerProviderStateMixin {
  late CheckInViewModel _viewModel;
  GoogleMapController? _mapController;
  Map<String, dynamic>? _suggestedRandomLocationData;
  final TextEditingController _descriptionController = TextEditingController();

  late AnimationController _animationController;
  late Animation<double> _scaleAnimation;
  late Animation<double> _fadeAnimation;

  @override
  void initState() {
    super.initState();
    _viewModel = sl<CheckInViewModel>();
    _viewModel.loadInitialData();
    _viewModel.addListener(_onViewModelChange);

    _animationController = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 800),
    );
    _scaleAnimation = Tween<double>(begin: 0.5, end: 1.0).animate(
      CurvedAnimation(parent: _animationController, curve: Curves.easeOutBack),
    );
    _fadeAnimation = Tween<double>(begin: 0.0, end: 1.0).animate(
      CurvedAnimation(parent: _animationController, curve: Curves.easeIn),
    );
  }

  @override
  void dispose() {
    _viewModel.removeListener(_onViewModelChange);
    _descriptionController.dispose();
    _animationController.dispose();
    super.dispose();
  }

  void _onViewModelChange() {
    if (_viewModel.currentLocation != null && _mapController != null) {
      _mapController!.animateCamera(CameraUpdate.newLatLng(
        _viewModel.currentLocation!,
      ));
    }
  }

  void _onMapCreated(GoogleMapController controller) {
    _mapController = controller;
  }

  Future<void> _generateAndShowRandomLocation() async {
    final randomLocationData = await _viewModel.checkInRandomLocation();
    if (randomLocationData != null) {
      setState(() {
        _suggestedRandomLocationData = randomLocationData;
      });
      final LatLng randomLatLng = randomLocationData['location'];
      _mapController?.animateCamera(CameraUpdate.newLatLngZoom(randomLatLng, 15.0));
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Không thể lấy vị trí hiện tại. Vui lòng thử lại.')),
      );
    }
  }

  Future<void> _checkDistanceAndCheckIn() async {
    if (_viewModel.currentLocation == null || _suggestedRandomLocationData == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Không thể xác định vị trí hoặc địa điểm check-in.')),
      );
      return;
    }

    final LatLng currentLatLng = _viewModel.currentLocation!;
    final LatLng targetLatLng = _suggestedRandomLocationData!['location'];

    final double distance = _viewModel.calculateDistance(currentLatLng, targetLatLng);

    if (distance <= 50) { // Dưới 50 mét
      final bool? confirm = await showDialog<bool>(
        context: context,
        builder: (BuildContext context) {
          return AlertDialog(
            title: const Text('Xác nhận Check-in'),
            content: Text('Bạn đang cách địa điểm ${distance.toStringAsFixed(2)} mét. Bạn có muốn check-in tại đây không?'),
            actions: <Widget>[
              TextButton(
                onPressed: () => Navigator.of(context).pop(false),
                child: const Text('Hủy'),
              ),
              TextButton(
                onPressed: () => Navigator.of(context).pop(true),
                child: const Text('Đồng ý'),
              ),
            ],
          );
        },
      );

      if (confirm == true) {
        await _takePictureAndSaveMemory(targetLatLng, _suggestedRandomLocationData!['safetyLevel'], _suggestedRandomLocationData!['locationName']);
      }
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Bạn cần đến gần địa điểm hơn (${distance.toStringAsFixed(2)}m) để check-in.')),
      );
    }
  }

  Future<void> _takePictureAndSaveMemory(LatLng location, String safetyLevel, String locationName) async {
    final Memory? newMemory = await _viewModel.takePictureAndSaveMemory(location, safetyLevel, locationName, context);
    if (newMemory != null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Kỷ niệm đã được lưu!')),
      );
      _showTreasureCardPopup(newMemory.locationName, newMemory.rarity);
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Không có ảnh được chụp hoặc không có mô tả.')),
      );
    }
  }

  void _showTreasureCardPopup(String locationName, String rarity) {
    _animationController.reset();
    _animationController.forward();

    showDialog(
      context: context,
      builder: (BuildContext context) {
        return ScaleTransition(
          scale: _scaleAnimation,
          child: FadeTransition(
            opacity: _fadeAnimation,
            child: AlertDialog(
              title: const Text('Bạn vừa nhận được thẻ bài!', textAlign: TextAlign.center),
              content: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Icon(Icons.card_giftcard, size: 100, color: _getRarityColor(rarity)), // Icon thay thế
                  const SizedBox(height: 16),
                  Text(
                    locationName,
                    style: const TextStyle(fontSize: 22, fontWeight: FontWeight.bold),
                    textAlign: TextAlign.center,
                  ),
                  const SizedBox(height: 8),
                  Text(
                    'Độ hiếm: $rarity',
                    style: TextStyle(fontSize: 18, color: _getRarityColor(rarity), fontWeight: FontWeight.bold),
                    textAlign: TextAlign.center,
                  ),
                  const SizedBox(height: 8),
                  const Text(
                    'Chúc mừng bạn đã khám phá một địa điểm mới!',
                    textAlign: TextAlign.center,
                  ),
                ],
              ),
              actions: <Widget>[
                TextButton(
                  onPressed: () => Navigator.of(context).pop(),
                  child: const Text('Tuyệt vời!'),
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  Color _getRarityColor(String rarity) {
    switch (rarity) {
      case 'Thường':
        return Colors.grey;
      case 'Hiếm':
        return Colors.blue;
      case 'Huyền thoại':
        return Colors.amber;
      default:
        return Colors.grey;
    }
  }

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider<CheckInViewModel>.value(
      value: _viewModel,
      child: Consumer<CheckInViewModel>(
        builder: (context, viewModel, child) {
          final currentLatLng = viewModel.currentLocation;
          final suggestedLatLng = _suggestedRandomLocationData?['location'];
          final suggestedLocationName = _suggestedRandomLocationData?['locationName'] ?? '...';
          final distanceToSuggested = (currentLatLng != null && suggestedLatLng != null)
              ? viewModel.calculateDistance(currentLatLng, suggestedLatLng) / 1000 // Convert to km
              : null;

          return Scaffold(
            appBar: AppBar(
              title: const Text('Check-in'),
            ),
            body: Column(
              children: [
                Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'Bạn đang ở gần: ${suggestedLocationName} (${distanceToSuggested?.toStringAsFixed(2) ?? 'N/A'} km)',
                        style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                      ),
                      const SizedBox(height: 8),
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceAround,
                        children: [
                          ElevatedButton.icon(
                            onPressed: () async {
                              if (suggestedLatLng != null) {
                                final url = 'https://www.google.com/maps/dir/?api=1&destination=${suggestedLatLng.latitude},${suggestedLatLng.longitude}';
                                if (await canLaunchUrl(Uri.parse(url))) {
                                  await launchUrl(Uri.parse(url));
                                } else {
                                  ScaffoldMessenger.of(context).showSnackBar(
                                    const SnackBar(content: Text('Không thể mở Google Maps.')),
                                  );
                                }
                              }
                            },
                            icon: const Icon(Icons.directions),
                            label: const Text('Chỉ đường'),
                          ),
                          ElevatedButton.icon(
                            onPressed: _generateAndShowRandomLocation,
                            icon: const Icon(Icons.refresh),
                            label: const Text('Làm mới GPS'), // Đổi tên thành Làm mới địa điểm
                          ),
                        ],
                      ),
                      const SizedBox(height: 8),
                      if (viewModel.currentLocation == null) // Thông báo nếu GPS yếu
                        const Text(
                          'Đang tìm vị trí GPS... Vui lòng đảm bảo GPS đã bật.',
                          style: TextStyle(color: Colors.orange, fontStyle: FontStyle.italic),
                        ),
                    ],
                  ),
                ),
                Expanded(
                  child: GoogleMap(
                    onMapCreated: _onMapCreated,
                    initialCameraPosition: CameraPosition(
                      target: viewModel.currentLocation ?? const LatLng(10.762622, 106.660172),
                      zoom: 15.0,
                    ),
                    myLocationEnabled: true,
                    myLocationButtonEnabled: true,
                    markers: viewModel.markers,
                  ),
                ),
                Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: Column(
                    children: [
                      TextField(
                        controller: _descriptionController,
                        decoration: const InputDecoration(
                          hintText: 'Viết cảm xúc của bạn...',
                          border: OutlineInputBorder(),
                        ),
                        maxLines: 3,
                      ),
                      const SizedBox(height: 16),
                      SizedBox(
                        width: double.infinity,
                        child: ElevatedButton.icon(
                          onPressed: _checkDistanceAndCheckIn,
                          icon: const Icon(Icons.check_circle),
                          label: const Text('ĐÃ ĐẾN NƠI - Check-in'),
                          style: ElevatedButton.styleFrom(
                            padding: const EdgeInsets.symmetric(vertical: 15),
                            textStyle: const TextStyle(fontSize: 18),
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          );
        },
      ),
    );
  }
}