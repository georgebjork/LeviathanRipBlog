import os
from supabase import create_client, Client
import json


def main(args):

    url: str = os.environ.get("SUPABASE_URL")
    key: str = os.environ.get("SUPABASE_KEY")
    supabase: Client = create_client(url, key)

    response = supabase.table('blog').select("*").execute()

    data = response.data

    return {"body" : data}