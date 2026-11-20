"""Check source integrity, module references and local documentation links."""
from pathlib import Path
import csv
import html
from html.parser import HTMLParser
import json
import re
import subprocess
import sys
from urllib.parse import unquote

ROOT = Path(__file__).resolve().parents[1]
failures = []

def check(condition, message):
    if not condition:
        failures.append(message)

def git(*args):
    result = subprocess.run(["git", "-C", str(ROOT), *args], capture_output=True, check=True)
    return result.stdout

public_cs = [p for p in git("ls-tree", "-r", "--name-only", "866b7446").decode().splitlines() if p.endswith(".cs")]
for relative in public_cs:
    original = git("show", "866b7446:" + relative)
    actual = (ROOT / relative).read_bytes()
    check(actual == original, "Public source changed: " + relative)

research_cs = {p.relative_to(ROOT).as_posix() for p in ROOT.rglob("*.cs")
               if not any(part in {"examples", ".git", "bin", "obj"} for part in p.relative_to(ROOT).parts)}
check(research_cs == set(public_cs), "Research source differs from the baseline file set")
example_projects = {p.parent.name for p in (ROOT / "examples").glob("[0-9]*/*.csproj")}
check(example_projects == {"01-space-time", "02-dynamic-update"}, "Unexpected example set")

with (ROOT / "specs/SOURCE_MODULES.csv").open(encoding="utf-8", newline="") as stream:
    mappings = list(csv.DictReader(stream))
check(len({row["source"] for row in mappings}) == len(mappings), "Duplicate source-module entries")
implemented = [row for row in mappings if row["status"] == "implementation"]
specified = [row for row in mappings if row["status"] == "specification"]
check({row["repository_path"] for row in implemented} == set(public_cs), "Source index does not cover the baseline C# files")
for row in mappings:
    check(row["status"] in {"implementation", "specification"},
          "Unknown module status: " + row["source"])
    if row["status"] == "implementation":
        check(bool(row["repository_path"]) and (ROOT / row["repository_path"]).is_file(), "Missing source file: " + row["source"])
    else:
        check(not row["repository_path"], "Specification-only module has a C# path: " + row["source"])
    for module in row["modules"].split(";"):
        check((ROOT / module).exists(), "Missing mapped algorithm: " + module)

class HtmlLinks(HTMLParser):
    def __init__(self):
        super().__init__()
        self.urls = []

    def handle_starttag(self, tag, attrs):
        self.urls.extend(value for name, value in attrs if name in {"href", "src"} and value)


def markdown_anchors(path):
    content = re.sub(r"```.*?```", "", path.read_text(encoding="utf-8-sig"), flags=re.S)
    anchors = set(re.findall(r'\bid=[\"\x27]([^\"\x27]+)', content))
    counts = {}
    for heading in re.findall(r"^#{1,6}\s+(.+)$", content, re.M):
        heading = re.sub(r"<[^>]+>", "", heading)
        heading = re.sub(r"\[([^]]+)\]\([^)]+\)", r"\1", heading)
        slug = re.sub(r"[^\w\- ]", "", html.unescape(heading).lower()).replace(" ", "-")
        count = counts.get(slug, 0)
        counts[slug] = count + 1
        anchors.add(slug + (f"-{count}" if count else ""))
    return anchors


links = 0
anchors_checked = 0
for doc in ROOT.rglob("*.md"):
    if any(part in {".git", "bin", "obj", "tmp"} for part in doc.relative_to(ROOT).parts):
        continue
    content = re.sub(r"```.*?```", "", doc.read_text(encoding="utf-8-sig"), flags=re.S)
    parsed = HtmlLinks()
    parsed.feed(content)
    urls = re.findall(r"\]\(([^)]+)\)", content) + parsed.urls
    for url in urls:
        url = url.strip().strip("<>").split(' "')[0]
        if re.match(r"^[a-zA-Z][a-zA-Z0-9+.-]*:", url) or url.startswith("//"):
            continue
        target = unquote(url.split("#")[0].split("?")[0])
        destination = doc.parent / target if target else doc
        if target:
            links += 1
            check(destination.exists(), f"Broken local link: {doc.relative_to(ROOT)} -> {url}")
        if "#" in url and destination.is_file() and destination.suffix == ".md":
            anchor = unquote(url.split("#", 1)[1])
            anchors_checked += 1
            check(anchor in markdown_anchors(destination), f"Broken heading link: {doc.relative_to(ROOT)} -> {url}")

pseudos = sorted(ROOT.glob("pseudocode/**/*.pseudo"))
for path in pseudos:
    content = path.read_text(encoding="utf-8-sig")
    for heading in ("PURPOSE", "INPUT", "OUTPUT"):
        check(re.search(r"^" + heading + r"\b", content, re.M), f"Missing {heading}: {path.name}")
    check("PROCEDURE " in content, "No procedure: " + path.name)

paper_rows = [line for line in (ROOT / "docs/PAPER_MAP.md").read_text(encoding="utf-8").splitlines()
              if re.match(r"\| \d+ \|", line)]
check([int(line.split("|")[1]) for line in paper_rows] == list(range(1, 41)), "Paper map must cover equations 1 through 40 exactly once")
check(all(len(line.split("|")) == 6 for line in paper_rows), "Malformed equation table row")

result = {
    "public_csharp_files": len(public_cs),
    "source_modules": len(mappings),
    "implementation_modules": len(implemented),
    "specification_only_modules": len(specified),
    "distributed_research_csharp_files": len(research_cs),
    "paper_equations_mapped": len(paper_rows),
    "example_projects": len(example_projects),
    "pseudocode_modules": len(pseudos),
    "local_links_checked": links,
    "heading_links_checked": anchors_checked,
    "failures": failures,
    "passed": not failures,
}
print(json.dumps(result, indent=2))
if "--write" in sys.argv:
    (ROOT / "validation/repository-checks.json").write_text(json.dumps(result, indent=2) + "\n", encoding="utf-8")
sys.exit(bool(failures))
