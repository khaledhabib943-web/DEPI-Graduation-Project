"""
Stitch HTML → ASP.NET .cshtml Converter
========================================
Converts your 33 Stitch HTML files into ready .cshtml files
with the correct folder structure for both Arabic and English versions.

OUTPUT STRUCTURE:
    Views/
    ├── Ar/
    │   ├── PageName/
    │   │   └── Index.cshtml
    │   └── ...
    └── En/
        ├── PageName/
        │   └── Index.cshtml
        └── ...

HOW TO USE:
    1. Place this script anywhere on your PC
    2. Run: python convert_to_cshtml.py
    3. When prompted, enter the path to your HTML files folder
    4. Copy the generated Views/Ar and Views/En folders into your .NET project
"""

import os
import re

# ── CONFIG ──────────────────────────────────────────────────────────────────

AR_LAYOUT = "~/Views/Shared/_Layout.cshtml"
EN_LAYOUT = "~/Views/Shared/_Layout.cshtml"

# Page title map — edit these to match your actual HTML folder names
# Key   = your HTML folder name (exact, lowercase)
# Value = (Arabic title, English title)
PAGE_TITLES = {
    "admin_dashboard": ("لوحة تحكم المدير", "Admin Dashboard"),
    "admin_dashboard_english": ("لوحة تحكم المدير", "Admin Dashboard"),

    "complaint_center": ("مركز الشكاوى", "Complaint Center"),
    "complaint_center_english": ("مركز الشكاوى", "Complaint Center"),

    "customer_dashboard": ("لوحة تحكم العميل", "Customer Dashboard"),
    "customer_dashboard_english": ("لوحة تحكم العميل", "Customer Dashboard"),

    "customer_profile_left_sidebar": ("الملف الشخصي", "Customer Profile"),
    "customer_profile_english_left_sidebar": ("الملف الشخصي", "Customer Profile"),

    "favorites_list": ("المفضلة", "Favorites List"),
    "favorites_list_english": ("المفضلة", "Favorites List"),

    "job_details_scheduling": ("تفاصيل المهمة وجدولتها", "Job Details & Scheduling"),
    "job_details_scheduling_english": ("تفاصيل المهمة وجدولتها", "Job Details & Scheduling"),

    "landing_page_home": ("الصفحة الرئيسية", "Home"),
    "landing_page_home_english": ("الصفحة الرئيسية", "Home"),

    "notification_center": ("مركز الإشعارات", "Notification Center"),
    "notification_center_english": ("مركز الإشعارات", "Notification Center"),

    "order_confirmation": ("تأكيد الطلب", "Order Confirmation"),
    "order_confirmation_english": ("تأكيد الطلب", "Order Confirmation"),

    "rate_service": ("تقييم الخدمة", "Rate Service"),
    "rate_service_english": ("تقييم الخدمة", "Rate Service"),

    "secure_login": ("تسجيل الدخول", "Secure Login"),
    "secure_login_english": ("تسجيل الدخول", "Secure Login"),

    "select_a_professional": ("اختيار مقدم خدمة", "Select a Professional"),
    "select_a_professional_english": ("اختيار مقدم خدمة", "Select a Professional"),

    "select_a_service": ("اختيار خدمة", "Select a Service"),
    "select_a_service_english": ("اختيار خدمة", "Select a Service"),

    "track_request_status": ("تتبع حالة الطلب", "Track Request Status"),
    "track_request_status_english": ("تتبع حالة الطلب", "Track Request Status"),

    "user_registration_customer_worker": ("تسجيل المستخدم", "User Registration"),
    "user_registration_english": ("تسجيل المستخدم", "User Registration"),

    "worker_dashboard": ("لوحة تحكم العامل", "Worker Dashboard"),
    "worker_dashboard_english": ("لوحة تحكم العامل", "Worker Dashboard"),

    "worker_profile_left_sidebar": ("ملف العامل", "Worker Profile"),
    "worker_profile_english_left_sidebar": ("ملف العامل", "Worker Profile"),
}

# ── HELPERS ─────────────────────────────────────────────────────────────────

def extract_all_styles(html: str) -> str:
    blocks = re.findall(r"<style[^>]*>(.*?)</style>", html, re.DOTALL | re.IGNORECASE)
    return "\n\n".join(b.strip() for b in blocks)

def extract_body(html: str) -> str:
    match = re.search(r"<body[^>]*>(.*?)</body>", html, re.DOTALL | re.IGNORECASE)
    return match.group(1).strip() if match else html.strip()

def to_pascal_case(name: str) -> str:
    return "".join(word.capitalize() for word in re.split(r"[-_]", name))

def build_cshtml(title: str, layout: str, css: str, body: str, lang: str) -> str:
    if lang == "ar":
        dir_attr  = 'dir="rtl"'
        extra_css = "    body { direction: rtl; font-family: 'Cairo', sans-serif; }\n"
    else:
        dir_attr  = 'dir="ltr"'
        extra_css = "    body { direction: ltr; }\n"

    return (
        f'@{{\n'
        f'    ViewData["Title"] = "{title}";\n'
        f'    Layout = "{layout}";\n'
        f'}}\n\n'
        f'<style>\n'
        f'{extra_css}\n'
        f'{css}\n'
        f'</style>\n\n'
        f'<div {dir_attr}>\n'
        f'{body}\n'
        f'</div>\n'
    )

# ── MAIN ────────────────────────────────────────────────────────────────────

def main():
    print("\n╔══════════════════════════════════════════════╗")
    print("║   Stitch HTML  →  .cshtml Converter          ║")
    print("╚══════════════════════════════════════════════╝\n")

    input_root = input("📁 Path to your HTML files folder:\n> ").strip().strip('"')
    if not os.path.isdir(input_root):
        print(f"\n❌ Folder not found: {input_root}")
        return

    output_root = input("\n📁 Where to save output (e.g. C:\\MyProject\\Views):\n> ").strip().strip('"')
    os.makedirs(output_root, exist_ok=True)

    ar_root = os.path.join(output_root, "Ar")
    en_root = os.path.join(output_root, "En")
    os.makedirs(ar_root, exist_ok=True)
    os.makedirs(en_root, exist_ok=True)

    folders = [f for f in os.listdir(input_root)
               if os.path.isdir(os.path.join(input_root, f))]

    if not folders:
        print("\n❌ No subfolders found.")
        return

    print(f"\n🔍 Found {len(folders)} page folder(s)...\n")
    success = 0
    skipped = 0

    for folder_name in sorted(folders):
        folder_path = os.path.join(input_root, folder_name)
        html_files  = [f for f in os.listdir(folder_path) if f.endswith(".html")]

        if not html_files:
            print(f"  ⚠️  Skipped '{folder_name}' — no .html file found")
            skipped += 1
            continue

        with open(os.path.join(folder_path, html_files[0]), "r", encoding="utf-8", errors="ignore") as f:
            html = f.read()

        css  = extract_all_styles(html)
        body = extract_body(html)

        ar_title, en_title = PAGE_TITLES.get(
            folder_name.lower(),
            (folder_name.replace("-", " ").title(),
             folder_name.replace("-", " ").title())
        )

        page_folder = to_pascal_case(folder_name)

        # Arabic output
        ar_dir = os.path.join(ar_root, page_folder)
        os.makedirs(ar_dir, exist_ok=True)
        with open(os.path.join(ar_dir, "Index.cshtml"), "w", encoding="utf-8") as f:
            f.write(build_cshtml(ar_title, AR_LAYOUT, css, body, "ar"))

        # English output
        en_dir = os.path.join(en_root, page_folder)
        os.makedirs(en_dir, exist_ok=True)
        with open(os.path.join(en_dir, "Index.cshtml"), "w", encoding="utf-8") as f:
            f.write(build_cshtml(en_title, EN_LAYOUT, css, body, "en"))

        print(f"  ✅ {folder_name:35s} → Ar/{page_folder}  +  En/{page_folder}")
        success += 1

    print(f"\n{'─'*55}")
    print(f"✅ Done! {success} pages converted, {skipped} skipped.")
    print(f"\n📂 Output saved to:")
    print(f"   {ar_root}")
    print(f"   {en_root}")
    print(f"\n💡 Next: Copy Ar/ and En/ folders into your Visual Studio")
    print(f"   project under Views/")
    print(f"{'─'*55}\n")

if __name__ == "__main__":
    main()
