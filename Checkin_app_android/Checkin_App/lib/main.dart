import 'package:flutter/material.dart';
import 'package:app_map/presentation/screens/home_screen.dart';
import 'package:app_map/presentation/screens/map_screen.dart';
import 'package:app_map/presentation/screens/check_in_screen.dart';
import 'package:app_map/presentation/screens/collection_screen.dart';
import 'package:app_map/presentation/screens/profile_screen.dart';
import 'package:app_map/core/di.dart' as di;
import 'package:app_map/domain/entities/memory.dart';
import 'package:app_map/database/database_helper.dart';
import 'package:shared_preferences/shared_preferences.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await di.init();
  await _addSampleData(); // Add sample data
  runApp(const MyApp());
}

Future<void> _addSampleData() async {
  final prefs = await SharedPreferences.getInstance();
  final bool hasSampleData = prefs.getBool('hasSampleData') ?? false;

  if (!hasSampleData) {
    final dbHelper = DatabaseHelper();

    final List<Memory> sampleMemories = [
      Memory(
        latitude: 10.7722,
        longitude: 106.6980,
        imagePath: '', // Placeholder, actual image not needed for display
        description: 'Khám phá Nhà thờ Đức Bà Sài Gòn.',
        timestamp: DateTime.now().subtract(const Duration(days: 5)),
        safetyLevel: 'An toàn',
        locationName: 'Nhà thờ Đức Bà',
        rarity: 'Hiếm',
      ),
      Memory(
        latitude: 10.7942,
        longitude: 106.7000,
        imagePath: '', // Placeholder
        description: 'Thăm quan Dinh Độc Lập.',
        timestamp: DateTime.now().subtract(const Duration(days: 10)),
        safetyLevel: 'An toàn',
        locationName: 'Dinh Độc Lập',
        rarity: 'Thường',
      ),
      Memory(
        latitude: 10.7680,
        longitude: 106.6950,
        imagePath: '', // Placeholder
        description: 'Dạo chơi ở Phố đi bộ Nguyễn Huệ.',
        timestamp: DateTime.now().subtract(const Duration(days: 15)),
        safetyLevel: 'An toàn',
        locationName: 'Phố đi bộ Nguyễn Huệ',
        rarity: 'Thường',
      ),
      Memory(
        latitude: 10.8000,
        longitude: 106.7000,
        imagePath: '', // Placeholder
        description: 'Trải nghiệm ẩm thực tại Chợ Bến Thành.',
        timestamp: DateTime.now().subtract(const Duration(days: 20)),
        safetyLevel: 'Hiếm',
        locationName: 'Chợ Bến Thành',
        rarity: 'Hiếm',
      ),
      Memory(
        latitude: 10.7800,
        longitude: 106.6800,
        imagePath: '', // Placeholder
        description: 'Ngắm cảnh hoàng hôn trên sông Sài Gòn.',
        timestamp: DateTime.now().subtract(const Duration(days: 25)),
        safetyLevel: 'Nguy hiểm',
        locationName: 'Sông Sài Gòn',
        rarity: 'Huyền thoại',
      ),
    ];

    for (var memory in sampleMemories) {
      await dbHelper.insertMemory(memory);
    }

    await prefs.setBool('hasSampleData', true);
    print('Sample data added to database.');
  }
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Check-in Ký Ức',
      theme: ThemeData(
        primarySwatch: Colors.blue,
      ),
      home: const MainScreen(),
    );
  }
}

class MainScreen extends StatefulWidget {
  const MainScreen({super.key});

  @override
  State<MainScreen> createState() => _MainScreenState();
}

class _MainScreenState extends State<MainScreen> {
  int _selectedIndex = 0;

  static final List<Widget> _widgetOptions = <Widget>[
    const HomeScreen(),
    const MapScreen(),
    const CheckInScreen(),
    const CollectionScreen(),
    const ProfileScreen(),
  ];

  void _onItemTapped(int index) {
    setState(() {
      _selectedIndex = index;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: _widgetOptions.elementAt(_selectedIndex),
      ),
      bottomNavigationBar: BottomNavigationBar(
        items: const <BottomNavigationBarItem>[
          BottomNavigationBarItem(
            icon: Icon(Icons.home),
            label: 'Trang chủ',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.map),
            label: 'Bản đồ',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.camera_alt),
            label: 'Check-in',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.collections),
            label: 'Bộ sưu tập',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.person),
            label: 'Cá nhân',
          ),
        ],
        currentIndex: _selectedIndex,
        selectedItemColor: Colors.blue,
        unselectedItemColor: Colors.grey,
        onTap: _onItemTapped,
        type: BottomNavigationBarType.fixed,
      ),
    );
  }
}