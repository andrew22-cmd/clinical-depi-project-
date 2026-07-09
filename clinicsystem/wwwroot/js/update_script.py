import re
import sys

def process_file(file_path):
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # Replace alert with toastr
    content = re.sub(r'alert\((.*?result\.message.*?)\);', r'if (result.success) { toastr.success(\1); } else { toastr.error(\1); }', content)
    content = re.sub(r'alert\("تم الحذف بنجاح"\);', r'toastr.success("تم الحذف بنجاح");', content)
    content = re.sub(r'alert\("فشل الحذف"\);', r'toastr.error("فشل الحذف");', content)
    content = re.sub(r'alert\("حدث خطأ"\);', r'toastr.error("حدث خطأ");', content)
    content = re.sub(r'alert\("حدث خطأ أثناء الحفظ"\);', r'toastr.error("حدث خطأ أثناء الحفظ");', content)
    content = re.sub(r'alert\("اختر موعد أولاً."\);', r'toastr.warning("اختر موعد أولاً.");', content)
    content = re.sub(r'alert\("اختر المريض"\);', r'toastr.warning("اختر المريض");', content)

    # For responses that return arrays but now return {success, data}
    # Pattern: const data = await response.json();
    # We replace it with logic that extracts data.data if it exists, and shows toastr if data.message exists.
    
    # Let's find occurrences of variable assignments from response.json()
    pattern = r'(const|let|var)\s+(\w+)\s*=\s*await\s+response\.json\(\);'
    
    def repl(m):
        keyword = m.group(1)
        varname = m.group(2)
        
        return f'''{keyword} {varname} = await response.json();
    if ({varname}.message) {{
        if ({varname}.success) toastr.success({varname}.message);
        else toastr.error({varname}.message);
    }}
    if ({varname}.data !== undefined) {varname} = {varname}.data;'''

    content = re.sub(pattern, repl, content)

    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(content)

if __name__ == "__main__":
    process_file(sys.argv[1])
