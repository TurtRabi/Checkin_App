import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class DirectionsDatasource {
  final String googleApiKey = "AIzaSyCVCxZBM-7tPZO5_l4n2IBFCt2vmc0HKJc"; // Sử dụng API Key của bạn

  Future<List<LatLng>> getDirections(LatLng origin, LatLng destination) async {
    print('DirectionsDatasource: Đang gọi Google Directions API.');
    final url = Uri.parse(
        'https://maps.googleapis.com/maps/api/directions/json?origin=${origin.latitude},${origin.longitude}&destination=${destination.latitude},${destination.longitude}&key=$googleApiKey');

    final response = await http.get(url);

    if (response.statusCode == 200) {
      print('DirectionsDatasource: Nhận được phản hồi 200 OK.');
      final data = json.decode(response.body);
      final List<LatLng> polylinePoints = [];

      if (data['routes'] != null && data['routes'].isNotEmpty) {
        print('DirectionsDatasource: Tìm thấy lộ trình.');
        final points = data['routes'][0]['overview_polyline']['points'];
        polylinePoints.addAll(_decodePolyline(points));
      } else {
        print('DirectionsDatasource: Không tìm thấy lộ trình.');
      }
      return polylinePoints;
    } else {
      print('DirectionsDatasource: Lỗi khi tải chỉ đường: ${response.statusCode}');
      throw Exception('Failed to load directions');
    }
  }

  List<LatLng> _decodePolyline(String polyline) {
    List<LatLng> points = [];
    int index = 0;
    int len = polyline.length;
    int lat = 0;
    int lng = 0;

    while (index < len) {
      int b;
      int shift = 0;
      int result = 0;
      do {
        b = polyline.codeUnitAt(index++) - 63;
        result |= (b & 0x1f) << shift;
        shift += 5;
      } while (b >= 0x20);
      int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
      lat += dlat;

      shift = 0;
      result = 0;
      do {
        b = polyline.codeUnitAt(index++) - 63;
        result |= (b & 0x1f) << shift;
        shift += 5;
      } while (b >= 0x20);
      int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
      lng += dlng;

      points.add(LatLng((lat / 1E5), (lng / 1E5)));
    }
    return points;
  }
}
