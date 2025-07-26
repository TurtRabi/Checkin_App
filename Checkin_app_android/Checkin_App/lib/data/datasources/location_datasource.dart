import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:location/location.dart';
import 'dart:math';

class LocationDatasource {
  final Location _location = Location();
  final List<String> _safetyLevels = ['An toàn', 'Cảnh báo', 'Nguy hiểm'];

  Future<LatLng?> getCurrentLocation() async {
    bool _serviceEnabled;
    PermissionStatus _permissionGranted;

    _serviceEnabled = await _location.serviceEnabled();
    if (!_serviceEnabled) {
      _serviceEnabled = await _location.requestService();
      if (!_serviceEnabled) {
        return null;
      }
    }

    _permissionGranted = await _location.hasPermission();
    if (_permissionGranted == PermissionStatus.denied) {
      _permissionGranted = await _location.requestPermission();
      if (_permissionGranted != PermissionStatus.granted) {
        return null;
      }
    }

    final locationData = await _location.getLocation();
    if (locationData.latitude != null && locationData.longitude != null) {
      return LatLng(locationData.latitude!, locationData.longitude!);
    }
    return null;
  }

  String _getRandomSafetyLevel() {
    final random = Random();
    return _safetyLevels[random.nextInt(_safetyLevels.length)];
  }

  Future<Map<String, dynamic>> generateRandomLocation(LatLng center, double radiusInKm) async {
    final random = Random();
    final double radiusInMeters = radiusInKm * 1000;
    const double R = 6371e3; // Earth's radius in meters

    // Generate a random distance within the radius
    final double distance = random.nextDouble() * radiusInMeters; // in meters

    // Generate a random bearing (0 to 360 degrees)
    final double bearing = random.nextDouble() * 360;

    final double latRad = _toRadians(center.latitude);
    final double lonRad = _toRadians(center.longitude);
    final double bearingRad = _toRadians(bearing);

    final double lat2Rad = asin(sin(latRad) * cos(distance / R) +
        cos(latRad) * sin(distance / R) * cos(bearingRad));

    final double lon2Rad = lonRad + atan2(sin(bearingRad) * sin(distance / R) * cos(latRad),
        cos(distance / R) - sin(latRad) * sin(lat2Rad));

    return {
      'location': LatLng(_toDegrees(lat2Rad), _toDegrees(lon2Rad)),
      'safetyLevel': _getRandomSafetyLevel(),
    };
  }

  double _toRadians(double degree) {
    return degree * pi / 180;
  }

  double _toDegrees(double radian) {
    return radian * 180 / pi;
  }
}
