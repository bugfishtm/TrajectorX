// GravitySimulation.cpp : Defines the entry point for the application.
//
#include <windows.h>
#include <windowsx.h>
#include "framework.h"
#define STB_IMAGE_IMPLEMENTATION
#include "stb_image.h"
#undef max
#undef min
#include "GravitySimulation.h"
#include "gravity_body.h"
#include <vector>
#include <string>
#define _USE_MATH_DEFINES
#include <cmath>
#include <algorithm>
#include <commctrl.h>
#include <GL/gl.h>
#include <GL/glu.h>
#pragma comment(lib, "opengl32.lib")
#pragma comment(lib, "glu32.lib")

// Define Max String for Classes on Buffer
#define MAX_LOADSTRING 100

// Define PI if not defined
double M_PI = 3.14159265358979323846;


RECT glAreaRect;
HGLRC g_hglrc = NULL;

std::vector<gravity_body> bodies;
double g_simTime = 0.0;             // Simulation time in seconds
double g_timeMultiplier = 1.0;      // How fast simulation time advances (1.0 = real time)

// Global Variables:
HINSTANCE hInst;                                // current instance
WCHAR szTitle[MAX_LOADSTRING];                  // The title bar text
WCHAR szWindowClass[MAX_LOADSTRING];            // the main window class name
gravity_body_vector g_cameraPos(0, 0, -1e11); // Camera position in 3D
gravity_body_vector g_cameraTarget(0, 0, 0);  // Where the camera is looking (optional, for future use)

// Forward declarations of functions included in this code module:
ATOM                MyRegisterClass(HINSTANCE hInstance);
BOOL                InitInstance(HINSTANCE, int);
LRESULT CALLBACK    WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK    About(HWND, UINT, WPARAM, LPARAM);

// Message handler for about box.
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);
    switch (message)
    {
    case WM_INITDIALOG:
        return (INT_PTR)TRUE;

    case WM_COMMAND:
        if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
        {
            EndDialog(hDlg, LOWORD(wParam));
            return (INT_PTR)TRUE;
        }
        break;
    }
    return (INT_PTR)FALSE;
}
// Utility to find the dominant gravitational body (focus) for a given position and mass
const gravity_body* find_dominant_body(
    const gravity_body_vector& pos,
    double my_mass,
    const std::vector<gravity_body>& bodies,
    const gravity_body* self = nullptr
) {
    // Step 1: Find the most massive body (for fallback)
    const gravity_body* most_massive = nullptr;
    double max_mass = 0.0;
    for (const auto& b : bodies) {
        if (&b == self) continue;
        if (b.mass > max_mass) {
            max_mass = b.mass;
            most_massive = &b;
        }
    }

    // Step 2: For each candidate, check if this body is inside its Hill sphere
    const gravity_body* best_parent = nullptr;





    double min_hill_radius = 1e100; // Smallest Hill sphere that contains us

    for (const auto& candidate : bodies) {
        if (&candidate == self) continue;
        if (candidate.mass <= 0) continue;
        if (candidate.mass <= my_mass) continue; // Only consider more massive bodies

        // Distance from this body to candidate
        gravity_body_vector diff = pos - candidate.position;
        double dist = diff.length();
        if (dist == 0) continue; // Can't orbit yourself

        // Find candidate's own parent (the most massive other body)
        const gravity_body* candidate_parent = nullptr;
        double candidate_parent_mass = 0.0;
        double candidate_a = 0.0;
        for (const auto& parent : bodies) {
            if (&parent == &candidate || &parent == self) continue;
            if (parent.mass > candidate_parent_mass) {
                candidate_parent = &parent;
                candidate_parent_mass = parent.mass;
                candidate_a = (candidate.position - parent.position).length();
            }
        }

        // Compute Hill radius
        double hill_radius = 1e100;
        if (candidate_parent && candidate_parent_mass > 0 && candidate_a > 0) {
            hill_radius = candidate_a * pow(candidate.mass / (3.0 * candidate_parent_mass), 1.0 / 3.0);
        }

        // If we're inside the candidate's Hill sphere, and it's the smallest so far, pick it
        if (dist < hill_radius && hill_radius < min_hill_radius) {
            best_parent = &candidate;
            min_hill_radius = hill_radius;
        }
    }




    // Step 3: Fallback to the most massive object (e.g., Sgr A*) if nothing else
    if (best_parent)
        return best_parent;
    else
        return most_massive;
}

void InitBodiesListView(HWND hDlg, bool reinit = false) {
    HWND hList = GetDlgItem(hDlg, IDC_LIST4);
    if (!hList) return;

    // Save scroll positions BEFORE update
    int hScrollPos = GetScrollPos(hList, SB_HORZ);
    int vScrollPos = GetScrollPos(hList, SB_VERT);

    // Save focused and selected items BEFORE update
    int focusedIndex = ListView_GetNextItem(hList, -1, LVNI_FOCUSED);
    int selectedIndex = ListView_GetNextItem(hList, -1, LVNI_SELECTED);

    // Disable redraw to prevent flicker during update
    SendMessage(hList, WM_SETREDRAW, FALSE, 0);

    // Set extended styles for full row select and grid lines (only once)
    if (!reinit) {
        ListView_SetExtendedListViewStyle(hList, LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES);

        // Remove all existing columns first (if any)
        while (ListView_DeleteColumn(hList, 0)) {}

        // Define columns
        LVCOLUMN lvc = {};
        lvc.mask = LVCF_TEXT | LVCF_WIDTH | LVCF_SUBITEM;
        lvc.cx = 120;  // wider for better visibility

        static const wchar_t* columns[] = {
            L"Name",
            L"Orbiting",
            L"Mass (kg)",
            L"Radius (m)",
            L"Position (Vector-X)",
            L"Position (Vector-Y)",
            L"Position (Vector-Z)",
            L"Velocity (Vector-X)",
            L"Velocity (Vector-Y)",
            L"Velocity (Vector-Z)",
            L"Rotation Angle (radians)",
            L"Rotation Speed (radians/s)",
            L"Rotation (Vector-X)",
            L"Rotation (Vector-Y)",
            L"Rotation (Vector-Z)"
        };

        for (int i = 0; i < _countof(columns); ++i) {
            lvc.iSubItem = i;
            lvc.pszText = const_cast<LPWSTR>(columns[i]);
            ListView_InsertColumn(hList, i, &lvc);
        }
    }

    // Clear all items
    ListView_DeleteAllItems(hList);

    // Insert bodies data rows
    for (size_t i = 0; i < bodies.size(); ++i) {
        const gravity_body& body = bodies[i];

        LVITEM lvi = {};
        lvi.mask = LVIF_TEXT;
        lvi.iItem = static_cast<int>(i);
        lvi.iSubItem = 0;

        // Convert std::string to std::wstring properly
        std::wstring nameW(body.name.begin(), body.name.end());
        lvi.pszText = const_cast<LPWSTR>(nameW.c_str());
        ListView_InsertItem(hList, &lvi);

        wchar_t buf[64];

#define SET_SUBITEM_TEXT(idx, fmt, val) \
            swprintf_s(buf, _countof(buf), fmt, val); \
            ListView_SetItemText(hList, i, idx, buf);

        const gravity_body& bodyx = bodies[i];
        const gravity_body* focus = find_dominant_body(body.position, body.mass, bodies, &bodyx);
        std::wstring orbitingW = (focus ? std::wstring(focus->name.begin(), focus->name.end()) : L"");
        ListView_SetItemText(hList, i, 1, const_cast<LPWSTR>(orbitingW.c_str()));

        SET_SUBITEM_TEXT(2, L"%.3f", body.mass);

        SET_SUBITEM_TEXT(3, L"%.3f", body.radius);

        SET_SUBITEM_TEXT(4, L"%.3f", body.position.x);
        SET_SUBITEM_TEXT(5, L"%.3f", body.position.y);
        SET_SUBITEM_TEXT(6, L"%.3f", body.position.z);

        SET_SUBITEM_TEXT(7, L"%.3f", body.velocity.x);
        SET_SUBITEM_TEXT(8, L"%.3f", body.velocity.y);
        SET_SUBITEM_TEXT(9, L"%.3f", body.velocity.z);

        SET_SUBITEM_TEXT(10, L"%.3f", body.rotation_angle);
        SET_SUBITEM_TEXT(11, L"%.3f", body.angular_speed);

        SET_SUBITEM_TEXT(12, L"%.3f", body.rotation_axis.x);
        SET_SUBITEM_TEXT(13, L"%.3f", body.rotation_axis.y);
        SET_SUBITEM_TEXT(14, L"%.3f", body.rotation_axis.z);

#undef SET_SUBITEM_TEXT
    }

    // Re-enable redraw
    SendMessage(hList, WM_SETREDRAW, TRUE, 0);

    // Restore selection and focus if valid
    if (selectedIndex >= 0 && selectedIndex < (int)bodies.size()) {
        ListView_SetItemState(hList, selectedIndex, LVIS_SELECTED, LVIS_SELECTED);
    }
    if (focusedIndex >= 0 && focusedIndex < (int)bodies.size()) {
        ListView_SetItemState(hList, focusedIndex, LVIS_FOCUSED, LVIS_FOCUSED);
        ListView_EnsureVisible(hList, focusedIndex, FALSE);
    }

    // Now scroll the view horizontally by hScrollPos pixels
    SendMessage(hList, LVM_SCROLL, hScrollPos, 0);
    // Restore scroll positions AFTER update
    SetScrollPos(hList, SB_HORZ, hScrollPos, TRUE);
    SetScrollPos(hList, SB_VERT, vScrollPos, TRUE);


    // Refresh the list view window to apply all changes smoothly
    InvalidateRect(hList, NULL, TRUE);
    UpdateWindow(hList);
}



INT_PTR CALLBACK ListBodies(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam) {
    switch (message) {
    case WM_INITDIALOG:
        InitBodiesListView(hDlg);
        SetTimer(hDlg, 1, 1000, NULL);
        return TRUE;
    case WM_TIMER:
        if (wParam == 1) {
            InitBodiesListView(hDlg, true);  // your update function that refreshes the list content
        }
    break;
    case WM_COMMAND:
        switch (LOWORD(wParam)) {
        case IDOK:
            KillTimer(hDlg, 1);
            EndDialog(hDlg, LOWORD(wParam));
            return TRUE;
        case IDCANCEL:
            KillTimer(hDlg, 1);
            EndDialog(hDlg, LOWORD(wParam));
            return TRUE;
        }
    break;
    case WM_NOTIFY:
    {
        LPNMHDR lpnmh = (LPNMHDR)lParam;
        if (lpnmh->idFrom == IDC_LIST4 && lpnmh->code == NM_DBLCLK) {
            HWND hList = GetDlgItem(hDlg, IDC_LIST4);
            int sel = ListView_GetNextItem(hList, -1, LVNI_SELECTED);
            if (sel >= 0 && sel < (int)bodies.size()) {
                // Set camera position to "look at" the selected body
                const gravity_body& target = bodies[sel];

                // Place camera a fixed distance behind the body along the z axis
                double cameraDistance = 1e10; // Adjust as needed for your scale
                g_cameraPos = target.position - gravity_body_vector(0, 0, cameraDistance);

                // Optionally, set camera target for future use (not needed for current projection)
                g_cameraTarget = target.position;

                // Redraw the main window (not the dialog!)
                HWND hMainWnd = GetParent(hDlg);
                if (!hMainWnd) hMainWnd = GetForegroundWindow(); // fallback
                InvalidateRect(hMainWnd, NULL, FALSE);
            }
            return TRUE;
        }
        break;
    }
    }
    return FALSE;
}


int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPWSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    // TODO: Place code here.

    INITCOMMONCONTROLSEX icex = { sizeof(icex), ICC_LISTVIEW_CLASSES };
    InitCommonControlsEx(&icex);
    // Initialize global strings
    LoadStringW(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
    LoadStringW(hInstance, IDC_GRAVITYSIMULATION, szWindowClass, MAX_LOADSTRING);
    MyRegisterClass(hInstance);

    // Perform application initialization:
    if (!InitInstance (hInstance, nCmdShow))
    {
        return FALSE;
    }
    HACCEL hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_GRAVITYSIMULATION));

    MSG msg;

    // Main message loop:
    while (GetMessage(&msg, nullptr, 0, 0))
    {
        if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
    }

    return (int) msg.wParam;
}


struct OrbitEllipse {
    double a;      // semi-major axis
    double b;      // semi-minor axis
    double e;      // eccentricity
    double omega;  // argument of periapsis (radians)
    gravity_body_vector center; // focus (central body position)
};

OrbitEllipse compute_orbit(const gravity_body& body, const gravity_body& central) {
    const double G = 6.67430e-11;
    double mu = G * (central.mass + body.mass);
    gravity_body_vector r = body.position - central.position;
    gravity_body_vector v = body.velocity - central.velocity;
    double r_len = r.length();
    double v_len = v.length();

    // Specific orbital energy
    double energy = (v_len * v_len) / 2.0 - mu / r_len;
    // Semi-major axis
    double a = -mu / (2.0 * energy);

    // Angular momentum (scalar in 2D)
    double h = r.x * v.y - r.y * v.x;

    // Eccentricity
    double e = sqrt(1.0 - (h * h) / (a * mu));

    // Argument of periapsis
    // Eccentricity vector
    gravity_body_vector ev = ((v * (v_len * v_len / mu)) - (r * (1.0 / r_len)));
    double omega = atan2(r.y * v_len * v_len - v.y * r_len * mu, r.x * v_len * v_len - v.x * r_len * mu);

    // Semi-minor axis
    double b = a * sqrt(1 - e * e);

    OrbitEllipse ellipse = { a, b, e, omega, central.position };
    return ellipse;
}

//
//  FUNCTION: MyRegisterClass()
//
//  PURPOSE: Registers the window class.
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
    WNDCLASSEXW wcex;

    wcex.cbSize = sizeof(WNDCLASSEX);

    //wcex.style          = CS_HREDRAW | CS_VREDRAW;
    wcex.style          = 0;
    wcex.lpfnWndProc    = WndProc;
    wcex.cbClsExtra     = 0;
    wcex.cbWndExtra     = 0;
    wcex.hInstance      = hInstance;
    wcex.hIcon          = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_GRAVITYSIMULATION));
    wcex.hCursor        = LoadCursor(nullptr, IDC_ARROW);
    wcex.hbrBackground  = (HBRUSH)(COLOR_WINDOW+1);
    wcex.lpszMenuName   = MAKEINTRESOURCEW(IDC_GRAVITYSIMULATION);
    wcex.lpszClassName  = szWindowClass;
    wcex.hIconSm        = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

    return RegisterClassExW(&wcex);
}





//
//   FUNCTION: InitInstance(HINSTANCE, int)
//
//   PURPOSE: Saves instance handle and creates main window
//
//   COMMENTS:
//
//        In this function, we save the instance handle in a global variable and
//        create and display the main program window.
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
   hInst = hInstance; // Store instance handle in our global variable

   HWND hWnd = CreateWindowW(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
      CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, nullptr, nullptr, hInstance, nullptr);

   if (!hWnd)
   {
      return FALSE;
   }

   ShowWindow(hWnd, nCmdShow);
   SetTimer(hWnd, 1, 16, NULL); // ~60 FPS (16 ms)
   UpdateWindow(hWnd);


   // Example: hardcoded, but in practice, load from file or UI
   struct BodyInit {
       const char* name;
       double mass;
       double x, y, z;
       double vx, vy, vz;
       double radius;
       double rot_ax_x, rot_ax_y, rot_ax_z;
       double angular_speed;
       double rotation_angle;
   };


   // All values are approximate and for demonstration only.
   // For best results, use JPL/Horizons for real epoch vectors.

// Data from NASA/JPL (J2000, ecliptic coordinates, approximate)
   BodyInit body_data[] = {
    //{ "SagittariusA*", 4.154e6 * 1.98847e30, 0, 0, 0, 0, 0, 0, 1.2e10 },

    // --- The Sun (galactocentric, J2000) ---
    // Sun is ~8.178 kpc from Sgr A*; 1 parsec = 3.085677581e16 m
    // Sun galactocentric position (Reid+ 2019): x = -8.178 kpc, y = 0, z = 0.02 kpc
    // Velocities: vx = 0, vy = 233 km/s (galactic rotation), vz = 7 km/s
    { "Sun", 1.98847e30, -8.178 * 3.085677581e19, 0, 0.02 * 3.085677581e19, 0, 233000, 7000, 6.9634e8 },

    // --- Solar System planets (positions/velocities relative to Sgr A*) ---
    // Mercury
    { "Mercury", 3.3011e23, -8.178 * 3.085677581e19 + 5.791e10, 0, 0.02 * 3.085677581e19, 0, 233000 + 47870, 7000, 2.4397e6 },
    // Venus
    { "Venus", 4.8675e24, -8.178 * 3.085677581e19 + 1.082e11, 0, 0.02 * 3.085677581e19, 0, 233000 + 35020, 7000, 6.0518e6 },
    // Earth
    { "Earth", 5.972e24, -8.178 * 3.085677581e19 + 1.496e11, 0, 0.02 * 3.085677581e19, 0, 233000 + 29780, 7000, 6.371e6 },
    // Moon
    { "Moon", 7.34767309e22, -8.178 * 3.085677581e19 + 1.496e11 + 3.844e8, 0, 0.02 * 3.085677581e19, 0, 233000 + 29780 + 1022, 7000, 1.7371e6 },
    // Mars
    { "Mars", 6.4171e23, -8.178 * 3.085677581e19 + 2.279e11, 0, 0.02 * 3.085677581e19, 0, 233000 + 24070, 7000, 3.3895e6 },
    // Jupiter
    { "Jupiter", 1.8982e27, -8.178 * 3.085677581e19 + 7.785e11, 0, 0.02 * 3.085677581e19, 0, 233000 + 13070, 7000, 6.9911e7 },
    // Saturn
    { "Saturn", 5.6834e26, -8.178 * 3.085677581e19 + 1.433e12, 0, 0.02 * 3.085677581e19, 0, 233000 + 9680, 7000, 5.8232e7 },
    // Uranus
    { "Uranus", 8.6810e25, -8.178 * 3.085677581e19 + 2.877e12, 0, 0.02 * 3.085677581e19, 0, 233000 + 6800, 7000, 2.5362e7 },
    // Neptune
    { "Neptune", 1.02413e26, -8.178 * 3.085677581e19 + 4.503e12, 0, 0.02 * 3.085677581e19, 0, 233000 + 5430, 7000, 2.4622e7 },
    // Pluto
    { "Pluto", 1.303e22, -8.178 * 3.085677581e19 + 5.906e12, 0, 0.02 * 3.085677581e19, 0, 233000 + 4740, 7000, 1.1883e6 },

    // --- Major moons (relative to Sgr A*) ---
    // Io (Jupiter)
    { "Io", 8.9319e22, -8.178 * 3.085677581e19 + 7.785e11 + 4.217e8, 0, 0.02 * 3.085677581e19, 0, 233000 + 13070 + 17320, 7000, 1.8216e6 },
    // Europa (Jupiter)
    { "Europa", 4.7998e22, -8.178 * 3.085677581e19 + 7.785e11 + 6.711e8, 0, 0.02 * 3.085677581e19, 0, 233000 + 13070 + 13740, 7000, 1.5608e6 },
    /*
    // --- Alpha Centauri A (real Gaia DR3, relative to Sgr A*) ---
    // Alpha Cen A is ~1.338 pc from Sun, so add to Sun's galactocentric position
    { "AlphaCenA", 2.188e30,
        -8.178 * 3.085677581e19 + 1.338 * 3.085677581e16, 0, 0.02 * 3.085677581e19,
        0, 233000 + (-22.4e3), 7000, 8.6e8 },

        // Proxima Centauri (real Gaia DR3, relative to Sgr A*)
        { "ProximaCen", 2.446e29,
            -8.178 * 3.085677581e19 + 1.301 * 3.085677581e16, 0, 0.02 * 3.085677581e19,
            0, 233000 + (-22.2e3), 7000, 1.0e8 },

            // Proxima Centauri b (exoplanet, real semi-major axis, velocity estimate)
            { "ProximaCen_b", 1.27 * 5.972e24,
                -8.178 * 3.085677581e19 + 1.301 * 3.085677581e16 + 7.5e9, 0, 0.02 * 3.085677581e19,
                0, 233000 + (-22.2e3) + 47.9e3, 7000, 6.4e6 },

                // Barnard's Star (real Gaia DR3, relative to Sgr A*)
                { "BarnardsStar", 1.44e29,
                    -8.178 * 3.085677581e19 + 1.834 * 3.085677581e17, 0, 0.02 * 3.085677581e19,
                    0, 233000 + (-110.6e3), 7000, 2.0e8 },

                    // Sirius A (real Gaia DR3, relative to Sgr A*)
                    { "SiriusA", 4.018e30,
                        -8.178 * 3.085677581e19 + 2.637 * 3.085677581e17, 0, 0.02 * 3.085677581e19,
                        0, 233000 + (-5.5e3), 7000, 1.19e9 },*/

   };


   bodies.clear();
   for (const auto& b : body_data) {
       bodies.push_back(gravity_body(
           b.name, b.mass,
           gravity_body_vector(b.x, b.y, b.z),
           gravity_body_vector(b.vx, b.vy, b.vz),
           b.radius,
           gravity_body_vector(b.rot_ax_x, b.rot_ax_y, b.rot_ax_z),
           b.angular_speed,
           b.rotation_angle
       ));
   }

   // --- OpenGL context creation ---
   HDC hdc = GetDC(hWnd);

   PIXELFORMATDESCRIPTOR pfd = {
       sizeof(PIXELFORMATDESCRIPTOR),
       1, // Version
       PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER,  // Enable double buffering
       PFD_TYPE_RGBA,
       32, // Color depth
       0, 0, 0, 0, 0, 0, // Ignore color bits
       24, // Depth buffer
       8,  // Stencil buffer
       0,  // Auxiliary buffers
       PFD_MAIN_PLANE,
       0, 0, 0, 0
   };
   pfd.dwFlags = PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER;
   pfd.iPixelType = PFD_TYPE_RGBA;
   pfd.cColorBits = 32;
   pfd.cDepthBits = 24;
   pfd.iLayerType = PFD_MAIN_PLANE;

   int pf = ChoosePixelFormat(hdc, &pfd);
   SetPixelFormat(hdc, pf, &pfd);

   g_hglrc = wglCreateContext(hdc);
   wglMakeCurrent(hdc, g_hglrc);

   ReleaseDC(hWnd, hdc);

   return TRUE;
}
void UpdateSimulation(double dt) {
    std::vector<gravity_body_vector> forces(bodies.size(), gravity_body_vector(0, 0, 0));

    // 1. Calculate gravitational forces between all pairs
    for (size_t i = 0; i < bodies.size(); ++i) {
        for (size_t j = 0; j < bodies.size(); ++j) {
            if (i == j) continue;
            gravity_body_vector force = bodies[i].gravitational_force(bodies[j]);
            forces[i] += force;
        }
    }

    // 2. Apply forces and update velocities
    for (size_t i = 0; i < bodies.size(); ++i) {
        bodies[i].apply_force(forces[i], dt);
    }

    // 3. Update positions and rotations
    for (auto& body : bodies) {
        body.update_position(dt);
        body.update_rotation(dt);
    }
}


// Example: Adding a new body at perihelion (or pericenter) for an elliptical orbit
void add_body_auto_orbit(
    const std::string& name,
    double mass,
    double a,      // semi-major axis
    double e,      // eccentricity
    double radius)
{
    double r_peri = a * (1 - e);
    gravity_body_vector pos(r_peri, 0, 0);

    // Find dominant body at this location
    const gravity_body* focus = find_dominant_body(pos, mass, bodies);
    double v_peri = 0.0;
    if (focus) {
        double G = 6.67430e-11;
        v_peri = sqrt(G * (focus->mass + mass) * (1 + e) / (a * (1 - e)));
    }
    gravity_body_vector vel(0, v_peri, 0);

    bodies.push_back(gravity_body(name, mass, pos, vel, radius));
}

GLuint LoadTexture(const char* filepath)
{
    int width, height, nrChannels;
    unsigned char* data = stbi_load(filepath, &width, &height, &nrChannels, 0);

    if (!data)
    {
        //std::cout << "Failed to load texture: " << filepath << std::endl;
        return 0;
    }

    GLuint textureID;
    glGenTextures(1, &textureID);
    glBindTexture(GL_TEXTURE_2D, textureID);

    // Set texture parameters (e.g., filtering and wrapping)
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR); // Mipmap filtering
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

    // Generate mipmaps manually using GLU (only needed for OpenGL 2.x and lower)
    gluBuild2DMipmaps(GL_TEXTURE_2D, GL_RGB, width, height, GL_RGB, GL_UNSIGNED_BYTE, data);

    stbi_image_free(data);

    return textureID;
}

void DrawSkybox(float size)
{
    glPushAttrib(GL_ENABLE_BIT);
    glDisable(GL_DEPTH_TEST);  // Disable depth test so it doesn't interfere with the scene
    glDisable(GL_LIGHTING);
    glDisable(GL_BLEND);

    GLuint texture_id = LoadTexture("Assets/Textures/skybox1.jpg");  // Load the single skybox texture

    glPushMatrix();
    glTranslatef(g_cameraPos.x, g_cameraPos.y, g_cameraPos.z);  // Move skybox with the camera

    glEnable(GL_TEXTURE_2D);
    glBindTexture(GL_TEXTURE_2D, texture_id);  // Bind the texture for all faces

    // Draw six faces of the cube
    glBegin(GL_QUADS);

    // Right face
    glTexCoord2f(0.0f, 0.0f); glVertex3f(size, -size, -size);
    glTexCoord2f(1.0f, 0.0f); glVertex3f(size, -size, size);
    glTexCoord2f(1.0f, 1.0f); glVertex3f(size, size, size);
    glTexCoord2f(0.0f, 1.0f); glVertex3f(size, size, -size);

    // Left face
    glTexCoord2f(0.0f, 0.0f); glVertex3f(-size, -size, size);
    glTexCoord2f(1.0f, 0.0f); glVertex3f(-size, -size, -size);
    glTexCoord2f(1.0f, 1.0f); glVertex3f(-size, size, -size);
    glTexCoord2f(0.0f, 1.0f); glVertex3f(-size, size, size);

    // Top face
    glTexCoord2f(0.0f, 0.0f); glVertex3f(-size, size, -size);
    glTexCoord2f(1.0f, 0.0f); glVertex3f(size, size, -size);
    glTexCoord2f(1.0f, 1.0f); glVertex3f(size, size, size);
    glTexCoord2f(0.0f, 1.0f); glVertex3f(-size, size, size);

    // Bottom face
    glTexCoord2f(0.0f, 0.0f); glVertex3f(-size, -size, -size);
    glTexCoord2f(1.0f, 0.0f); glVertex3f(size, -size, -size);
    glTexCoord2f(1.0f, 1.0f); glVertex3f(size, -size, size);
    glTexCoord2f(0.0f, 1.0f); glVertex3f(-size, -size, size);

    // Front face
    glTexCoord2f(0.0f, 0.0f); glVertex3f(-size, -size, size);
    glTexCoord2f(1.0f, 0.0f); glVertex3f(size, -size, size);
    glTexCoord2f(1.0f, 1.0f); glVertex3f(size, size, size);
    glTexCoord2f(0.0f, 1.0f); glVertex3f(-size, size, size);

    // Back face
    glTexCoord2f(0.0f, 0.0f); glVertex3f(size, -size, -size);
    glTexCoord2f(1.0f, 0.0f); glVertex3f(-size, -size, -size);
    glTexCoord2f(1.0f, 1.0f); glVertex3f(-size, size, -size);
    glTexCoord2f(0.0f, 1.0f); glVertex3f(size, size, -size);

    glEnd();  // End drawing

    glDisable(GL_TEXTURE_2D);
    glPopMatrix();
    glPopAttrib();
}

void RenderText(HDC hdc, const char* text, int x, int y)
{
    // Convert the const char* string to a wide-character string (securely)
    std::wstring wideText(text, text + strlen(text));  // Using wstring constructor for safe conversion

    // Set text color to white
    SetTextColor(hdc, RGB(255, 255, 255));

    // Make background transparent
    SetBkMode(hdc, TRANSPARENT);

    // Output text using wide-character version of TextOut
    TextOutW(hdc, x, y, wideText.c_str(), wideText.length());  // Use TextOutW for wide strings
}


void RestorePerspective()
{
    glMatrixMode(GL_PROJECTION);
    glPopMatrix();
    glMatrixMode(GL_MODELVIEW);
    glPopMatrix();
    glEnable(GL_DEPTH_TEST);  // Re-enable depth testing
}

void SetViewportToWindow(HDC hdc, int window_width, int window_height)
{
    glMatrixMode(GL_PROJECTION);
    glPushMatrix();
    glLoadIdentity();
    glOrtho(0.0, window_width, window_height, 0.0, -1.0, 1.0); // Ortho projection
    glMatrixMode(GL_MODELVIEW);
    glPushMatrix();
    glLoadIdentity();
    glDisable(GL_DEPTH_TEST);  // Disable depth testing while rendering 2D text
    glDisable(GL_LIGHTING);    // Disable lighting
}


//
//  FUNCTION: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE: Processes messages for the main window.
//
//  WM_COMMAND  - process the application menu
//  WM_PAINT    - Paint the main window
//  WM_DESTROY  - post a quit message and return
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
    case WM_ERASEBKGND:
        // Prevent default background erase to reduce flicker
        return 1;
    case WM_COMMAND:
        {
            int wmId = LOWORD(wParam);
            // Parse the menu selections:
            switch (wmId)
            {
            case IDM_ABOUT:
                DialogBox(hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, About);
                break;
            case ID_INVESTIGATION_BODIES:
                DialogBox(hInst, MAKEINTRESOURCE(IDD_BODIESBOX), hWnd, ListBodies);
                break;
            case IDM_EXIT:
                DestroyWindow(hWnd);
                break;
            default:
                return DefWindowProc(hWnd, message, wParam, lParam);
            }
        }
        break;
        // Rendering the OpenGL Scene and 2D Text UI
    case WM_PAINT:
    {
        PAINTSTRUCT ps;
        HDC hdc = BeginPaint(hWnd, &ps);  // Prepare HDC for rendering

        wglMakeCurrent(hdc, g_hglrc);  // Make OpenGL context current

        RECT clientRect;
        GetClientRect(hWnd, &clientRect);
        int window_width = clientRect.right - clientRect.left;
        int window_height = clientRect.bottom - clientRect.top;

        glViewport(0, 0, window_width, window_height);  // Set OpenGL viewport

        glClearColor(0, 0, 0, 1);  // Set background color to black
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);  // Clear the color and depth buffers
        glEnable(GL_DEPTH_TEST);  // Enable depth testing for 3D rendering

        // OpenGL rendering code here...
        glMatrixMode(GL_PROJECTION);
        glLoadIdentity();
        gluPerspective(45.0, (double)window_width / window_height, 1e9, 1e20);  // Set perspective projection

        glMatrixMode(GL_MODELVIEW);
        glLoadIdentity();
        gluLookAt(
            g_cameraPos.x, g_cameraPos.y, g_cameraPos.z,
            g_cameraTarget.x, g_cameraTarget.y, g_cameraTarget.z,
            0, 1, 0
        );

        // Render 3D scene, e.g., Draw Skybox and Sphere
        //DrawSkybox(5000);
        glColor3f(1.0f, 0.0f, 0.0f);  // Red color for sphere
        glPushMatrix();
        GLUquadric* quad = gluNewQuadric();
        gluSphere(quad, 10.0f, 32, 24);  // Draw sphere with radius 10.0f
        gluDeleteQuadric(quad);
        glPopMatrix();

        // Now render 2D UI (e.g., text) on top of OpenGL view
        glDisable(GL_DEPTH_TEST);  // Disable depth test for 2D UI rendering
        glMatrixMode(GL_PROJECTION);
        glLoadIdentity();
        glOrtho(0, window_width, window_height, 0, -1, 1);  // 2D orthographic projection
        glMatrixMode(GL_MODELVIEW);
        glLoadIdentity();

        // Render UI text (e.g., Camera Position)
        std::string cameraPositionText = "Camera Position: (" +
            std::to_string(g_cameraPos.x) + ", " +
            std::to_string(g_cameraPos.y) + ", " +
            std::to_string(g_cameraPos.z) + ")";

        RenderText(hdc, cameraPositionText.c_str(), window_width - 200, 20);

        // Restore OpenGL settings for 3D rendering
        glMatrixMode(GL_PROJECTION);
        glLoadIdentity();
        gluPerspective(45.0, (double)window_width / window_height, 1e9, 1e20);  // Restore perspective projection
        glMatrixMode(GL_MODELVIEW);
        glLoadIdentity();
        glEnable(GL_DEPTH_TEST);  // Re-enable depth test for 3D rendering

        SwapBuffers(hdc);  // Swap buffers to display the rendered scene
        EndPaint(hWnd, &ps);  // End painting
        break;
    }








    case 34654635434:
    {
        PAINTSTRUCT ps;
        HDC hdc = BeginPaint(hWnd, &ps);
        wglMakeCurrent(hdc, g_hglrc);
        glDisable(GL_LIGHTING);
        RECT clientRect;
        GetClientRect(hWnd, &clientRect);
        int window_width = clientRect.right - clientRect.left;
        int window_height = clientRect.bottom - clientRect.top;
        int centerX = window_width / 2;
        int centerY = window_height / 2;

        // --- Set up OpenGL viewport and projection ---
        glViewport(0, 0, window_width, window_height);
        glMatrixMode(GL_PROJECTION);
        glLoadIdentity();
        gluPerspective(45.0, (double)window_width / window_height, 1.0, 1e20);

        glMatrixMode(GL_MODELVIEW);
        glLoadIdentity();
        gluLookAt(g_cameraPos.x, g_cameraPos.y, g_cameraPos.z,
            g_cameraTarget.x, g_cameraTarget.y, g_cameraTarget.z,
            0, 1, 0);

        glClearColor(0, 0, 0, 1);
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
        glEnable(GL_DEPTH_TEST);
        //glEnable(GL_LIGHTING);
        //glEnable(GL_LIGHT0);

        // --- Draw orbits (as 3D line loops) ---
        glDisable(GL_LIGHTING);
        for (size_t i = 0; i < bodies.size(); ++i) {
            const auto& body = bodies[i];
            const gravity_body* focus = find_dominant_body(body.position, body.mass, bodies);
            if (!focus) continue;

            OrbitEllipse orbit = compute_orbit(body, *focus);
            const int N = 200;
            glColor3ub(128, 128, 255);
            glBegin(GL_LINE_LOOP);
            for (int j = 0; j < N; ++j) {
                double theta = 2.0 * M_PI * j / N;
                double x = orbit.a * cos(theta);
                double y = orbit.b * sin(theta);

                double xr = cos(orbit.omega) * x - sin(orbit.omega) * y;
                double yr = sin(orbit.omega) * x + cos(orbit.omega) * y;

                double px = orbit.center.x + xr;
                double py = orbit.center.y + yr;
                double pz = orbit.center.z;
                glVertex3d(px, py, pz);
            }
            glEnd();
        }
        //glEnable(GL_LIGHTING);

        // --- Draw bodies as real 3D spheres ---
        for (const auto& body : bodies) {
            glPushMatrix();
            glTranslated(body.position.x, body.position.y, body.position.z);
            GLfloat color[] = { 0.7f, 0.7f, 1.0f, 1.0f };
            glMaterialfv(GL_FRONT, GL_AMBIENT_AND_DIFFUSE, color);
            GLUquadric* quad = gluNewQuadric();
            gluSphere(quad, body.radius, 32, 24);
            gluDeleteQuadric(quad);
            glPopMatrix();
        }

        // --- Draw Schwarzschild radii as red wireframe spheres ---
        glDisable(GL_LIGHTING);
        glColor3ub(255, 0, 0);
        for (const auto& body : bodies) {
            double G = 6.67430e-11;
            double c = 299792458.0;
            double schwarzschild_radius = 2.0 * G * body.mass / (c * c);
            if (schwarzschild_radius > 0) {
                glPushMatrix();
                glTranslated(body.position.x, body.position.y, body.position.z);
                GLUquadric* quad = gluNewQuadric();
                gluQuadricDrawStyle(quad, GLU_LINE); // Wireframe
                gluSphere(quad, schwarzschild_radius, 32, 16);
                gluDeleteQuadric(quad);
                glPopMatrix();
            }
        }
        //glEnable(GL_LIGHTING);

        // --- 2D Overlay for scale bar and labels ---
        glMatrixMode(GL_PROJECTION);
        glPushMatrix();
        glLoadIdentity();
        glOrtho(0, window_width, window_height, 0, -1, 1);
        glMatrixMode(GL_MODELVIEW);
        glPushMatrix();
        glLoadIdentity();

        // Draw scale bar (white line)
        double nice_lengths[] = {
            1e6, 2e6, 5e6, 1e7, 2e7, 5e7, 1e8, 2e8, 5e8, 1e9, 2e9, 5e9, 1e10, 2e10, 5e10, 1e11
        };
        double scale_length = nice_lengths[0];
        for (double len : nice_lengths) {
            int px = static_cast<int>(len);
            //int px = static_cast<int>(len * g_viewScale);
            if (px > window_width / 4 && px < window_width / 2) {
                scale_length = len;
                break;
            }
        }
        //int px_length = static_cast<int>(scale_length * g_viewScale);
        int px_length = static_cast<int>(scale_length);
        int y = window_height - 40;
        int x1 = centerX - px_length / 2;
        int x2 = centerX + px_length / 2;

        glDisable(GL_LIGHTING);
        glColor3ub(255, 255, 255);
        glBegin(GL_LINES);
        glVertex2i(x1, y);
        glVertex2i(x2, y);
        glEnd();

        // Draw scale label (use a bitmap font or stb_easy_font for real text)
        char label[64];
        if (scale_length >= 1e9)
            sprintf_s(label, "%.0f million km", scale_length / 1e9);
        else
            sprintf_s(label, "%.0f km", scale_length / 1e3);
        // Example: draw text at (centerX - 40, y - 25)
        // Use a bitmap font function here, e.g. glutBitmapCharacter or stb_easy_font

        // Draw simulation time and speed
        extern double g_simTime, g_timeMultiplier;
        char timeLabel[128];
        int days = (int)(g_simTime / 86400);
        int hours = (int)((g_simTime - days * 86400) / 3600);
        int minutes = (int)((g_simTime - days * 86400 - hours * 3600) / 60);
        int seconds = (int)(g_simTime) % 60;
        sprintf_s(timeLabel, "Sim Time: %d days, %02d:%02d:%02d   Speed: %.0fx", days, hours, minutes, seconds, g_timeMultiplier);
        // Example: draw text at (20, 20)
        // Use a bitmap font function here

        // --- End 2D overlay ---
        glMatrixMode(GL_MODELVIEW);
        glPopMatrix();
        glMatrixMode(GL_PROJECTION);
        glPopMatrix();

        // --- Swap buffers to display ---
        SwapBuffers(hdc);

        EndPaint(hWnd, &ps);
        break;
    }

    break;


    case WM_KEYDOWN:
    {
        ///////////////////////////////////////////////////////////////////////
        // + / - Change the Time Stretch
        ///////////////////////////////////////////////////////////////////////
        if (wParam == VK_OEM_PLUS || wParam == VK_ADD) {
            g_timeMultiplier *= 2.0; // Double the speed
            if (g_timeMultiplier > 1e6) g_timeMultiplier = 1e6;
        }
        else if (wParam == VK_OEM_MINUS || wParam == VK_SUBTRACT) {
            g_timeMultiplier /= 2.0; // Halve the speed
            if (g_timeMultiplier < 1e-6) g_timeMultiplier = 1e-6;
        }

        ///////////////////////////////////////////////////////////////////////
        // W/A/S/D Move Starfield
        ///////////////////////////////////////////////////////////////////////
       // double panAmount = 1.0 / g_viewScale * 50; // Adjust 50 for pan speed (simulation units)
        switch (wParam) {
        case 'A': // Left
            //g_viewOffsetX -= panAmount;
            InvalidateRect(hWnd, NULL, FALSE);
            break;
        case 'D': // Right
            //g_viewOffsetX += panAmount;
            InvalidateRect(hWnd, NULL, FALSE);
            break;
        case 'W': // Up
            //g_viewOffsetY += panAmount;
            InvalidateRect(hWnd, NULL, FALSE);
            break;
        case 'S': // Down
           // g_viewOffsetY -= panAmount;
            InvalidateRect(hWnd, NULL, FALSE);
            break;
        }
    }
    break;
    case WM_DESTROY:
        ///////////////////////////////////////////////////////////////////////
        // Override for better UI Performance
        ///////////////////////////////////////////////////////////////////////
        if (g_hglrc) {
            wglMakeCurrent(NULL, NULL);
            wglDeleteContext(g_hglrc);
            g_hglrc = NULL;
        }
        PostQuitMessage(0);
        break;
    case WM_MOUSEWHEEL:
    {
        int delta = GET_WHEEL_DELTA_WPARAM(wParam);

        // Calculate view direction (from camera to target)
        gravity_body_vector viewDir = (g_cameraTarget - g_cameraPos).normalized();

        // Calculate zoom step (adjust this value for your simulation scale)
        double zoomStep = ((g_cameraTarget - g_cameraPos).length()) * 0.1; // 10% of current distance

        if (delta > 0) {
            // Zoom in: move camera forward
            g_cameraPos = g_cameraPos + viewDir * zoomStep;
        }
        else if (delta < 0) {
            // Zoom out: move camera backward
            g_cameraPos = g_cameraPos - viewDir * zoomStep;
        }

        // Optional: Clamp min/max distance to avoid flipping or going inside objects
        double minDist = 1e6, maxDist = 1e16;
        double dist = (g_cameraTarget - g_cameraPos).length();
        if (dist < minDist) g_cameraPos = g_cameraTarget - viewDir * minDist;
        if (dist > maxDist) g_cameraPos = g_cameraTarget - viewDir * maxDist;

        InvalidateRect(hWnd, &glAreaRect, FALSE);
        break;
    }


    case WM_TIMER:
    {   
        ///////////////////////////////////////////////////////////////////////
        // Calculate the time elapsed since the last update in seconds.
        ///////////////////////////////////////////////////////////////////////
        double dt = g_timeMultiplier * 0.016;
        g_simTime += dt;
        UpdateSimulation(dt);
        InvalidateRect(hWnd, &glAreaRect, FALSE); // Request a redraw
        break;
    }
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}

