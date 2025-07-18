import 'package:google_maps_flutter/google_maps_flutter.dart';

abstract class LocationRepository {
  Future<LatLng?> getCurrentLocation();
  Future<Map<String, dynamic>> generateRandomLocation(LatLng center, double radiusInKm);
}
