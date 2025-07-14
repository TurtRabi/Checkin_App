import 'package:google_maps_flutter/google_maps_flutter.dart';

abstract class DirectionsRepository {
  Future<List<LatLng>> getDirections(LatLng origin, LatLng destination);
}
